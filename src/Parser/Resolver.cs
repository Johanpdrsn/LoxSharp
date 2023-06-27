using LoxSharp.Model;

namespace LoxSharp.Parser;

internal class Resolver : Expr.Visitor<object?>, Stmt.Visitor<object?>
{
    private readonly Interpreter _interpreter;
    private readonly Stack<Dictionary<string, bool>> _scopes = new();
    private FunctionType _functionType = FunctionType.NONE;
    private ClassType _classType = ClassType.NONE;

    public Resolver(Interpreter interpreter)
    {
        _interpreter = interpreter;
    }

    private enum FunctionType
    {
        NONE,
        FUNCTION,
        INITIALIZER,
        METHOD
    }

    private enum ClassType
    {
        NONE,
        CLASS,
        SUBCLASS
    }

    internal void Resolve(List<Stmt> statements)
    {
        foreach (Stmt statement in statements)
        {
            Resolve(statement);
        }
    }

    private void Resolve(Stmt statement)
    {
        statement.Accept(this);
    }

    private void Resolve(Expr expr)
    {
        expr.Accept(this);
    }

    private void ResolveFunction(Stmt.Function function, FunctionType functionType)
    {
        FunctionType enclosingFunctionType = _functionType;
        _functionType = functionType;
        BeginScope();
        foreach (Token param in function.parameters)
        {
            Declare(param);
            Define(param);
        }
        Resolve(function.body);
        EndScope();
        _functionType = enclosingFunctionType;
    }

    private void BeginScope()
    {
        _scopes.Push(new Dictionary<string, bool>());
    }

    private void EndScope()
    {
        _scopes.Pop();
    }

    private void Declare(Token name)
    {
        if (_scopes.Count == 0) return;

        Dictionary<string, bool> scope = _scopes.Peek();
        if (scope.ContainsKey(name.lexeme))
        {
            LoxSharp.Error(name, "ALready a variable with this name in this scope.");
        }

        scope.Add(name.lexeme, false);
    }

    private void Define(Token name)
    {
        if (_scopes.Count == 0) return;
        _scopes.Peek()[name.lexeme] = true;
    }

    private void ResolveLocal(Expr expr, Token name)
    {
        for (int i = 0; i < _scopes.Count; i++)
        {
            if (_scopes.ElementAt(i).ContainsKey(name.lexeme))
            {
                _interpreter.Resolve(expr, i);
                return;
            }
        }
    }

    public object? VisitAssignExpr(Expr.Assign expr)
    {
        Resolve(expr.value);
        ResolveLocal(expr, expr.name);
        return null;
    }

    public object? VisitBinaryExpr(Expr.Binary expr)
    {
        Resolve(expr.left);
        Resolve(expr.right);
        return null;
    }

    public object? VisitBlockStmt(Stmt.Block stmt)
    {
        BeginScope();
        Resolve(stmt.statements);
        EndScope();
        return null;
    }

    public object? VisitCallExpr(Expr.Call expr)
    {
        Resolve(expr.callee);
        foreach (Expr arg in expr.arguments)
        {
            Resolve(arg);
        }
        return null;
    }

    public object? VisitExpressionStmt(Stmt.Expression stmt)
    {
        Resolve(stmt.expression);
        return null;
    }

    public object? VisitFunctionStmt(Stmt.Function stmt)
    {
        Declare(stmt.name);
        Define(stmt.name);

        ResolveFunction(stmt, FunctionType.FUNCTION);
        return null;
    }

    public object? VisitGroupingExpr(Expr.Grouping expr)
    {
        Resolve(expr.expression);
        return null;
    }

    public object? VisitIfStmt(Stmt.If stmt)
    {
        Resolve(stmt.condition);
        Resolve(stmt.thenBranch);
        if (stmt.elseBranch is not null)
            Resolve(stmt.elseBranch);
        return null;
    }

    public object? VisitLiteralExpr(Expr.Literal expr)
    {
        return null;
    }

    public object? VisitLogicalExpr(Expr.Logical expr)
    {
        Resolve(expr.left);
        Resolve(expr.right);
        return null;
    }

    public object? VisitPrintStmt(Stmt.Print stmt)
    {
        Resolve(stmt.expression);
        return null;
    }

    public object? VisitReturnStmt(Stmt.Return stmt)
    {
        if (_functionType is FunctionType.NONE)
            LoxSharp.Error(stmt.keyword, "Can't return from top level code.");

        if (stmt.value is not null)
        {
            if (_functionType is FunctionType.INITIALIZER)
                LoxSharp.Error(stmt.keyword, "Can't return from an initializer");

            Resolve(stmt.value);
        }

        return null;
    }

    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        Resolve(expr.right);
        return null;
    }

    public object? VisitVariableExpr(Expr.Variable expr)
    {
        if (_scopes.Count > 0 && _scopes.Peek().TryGetValue(expr.name.lexeme, out bool val) && !val)
        {
            LoxSharp.Error(expr.name, "Can't read local variable in its own initializer.");
        }
        ResolveLocal(expr, expr.name);
        return null;
    }

    public object? VisitVarStmt(Stmt.Var stmt)
    {
        Declare(stmt.name);
        if (stmt.initializer is not null)
        {
            Resolve(stmt.initializer);
        }
        Define(stmt.name);
        return null;
    }

    public object? VisitWhileStmt(Stmt.While stmt)
    {
        Resolve(stmt.condition);
        Resolve(stmt.body);
        return null;
    }

    public object? VisitClassStmt(Stmt.Class stmt)
    {
        ClassType enclosingClass = _classType;
        _classType = ClassType.CLASS;

        Declare(stmt.name);
        Define(stmt.name);

        if (stmt.superClass is not null && stmt.name.lexeme.Equals(stmt.superClass.name.lexeme))
        {
            LoxSharp.Error(stmt.superClass.name, "A class can't inherit from itself.");
        }
        if (stmt.superClass is not null)
        {
            _classType = ClassType.SUBCLASS;
            Resolve(stmt.superClass);
        }

        if (stmt.superClass is not null)
        {
            BeginScope();
            _scopes.Peek().Add("super", true);
        }

        BeginScope();
        _scopes.Peek()["this"] = true;

        foreach (Stmt.Function method in stmt.methods)
        {
            FunctionType declaration = FunctionType.METHOD;
            if (method.name.lexeme.Equals("init"))
                declaration = FunctionType.INITIALIZER;

            ResolveFunction(method, declaration);
        }

        EndScope();
        if (stmt.superClass is not null) EndScope();

        _classType = enclosingClass;
        return null;
    }

    public object? VisitGetExpr(Expr.Get expr)
    {
        Resolve(expr.obj);
        return null;
    }

    public object? VisitSetExpr(Expr.Set expr)
    {
        Resolve(expr.value);
        Resolve(expr.obj);
        return null;
    }

    public object? VisitThisExpr(Expr.This expr)
    {
        if (_classType is ClassType.NONE)
        {
            LoxSharp.Error(expr.keyword, "Can't use 'this' outside of class.");
        }
        ResolveLocal(expr, expr.keyword);
        return null;
    }

    public object? VisitSuperExpr(Expr.Super expr)
    {
        if (_classType is ClassType.NONE)
        {
            LoxSharp.Error(expr.keyword, "Can't use 'super' outside of a class");
        }
        else if (_classType is not ClassType.SUBCLASS)
        {
            LoxSharp.Error(expr.keyword, "Can't use 'super' in a class with no superclass");
        }
        ResolveLocal(expr, expr.keyword);
        return null;
    }
}

