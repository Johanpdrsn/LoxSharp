using LoxSharp.Error;
using LoxSharp.Interface;
using LoxSharp.Model;
using LoxSharp.NativeFunctions;
using Environment = LoxSharp.Model.Environment;

namespace LoxSharp;

public class Interpreter : Expr.Visitor<object?>, Stmt.Visitor<object?>
{
    internal readonly Environment globals = new();
    private readonly Dictionary<Expr, int> _locals = new();
    private Environment _environment;

    public Interpreter()
    {
        _environment = globals;
        globals.Define("Clock", new Clock());
    }

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (Stmt stmt in statements)
            {
                Execute(stmt);
            }
        }
        catch (RuntimeError ex)
        {
            LoxSharp.RuntimeError(ex);
        }
    }

    public object? VisitBinaryExpr(Expr.Binary expr)
    {
        object? left = Evaluate(expr.left);
        object? right = Evaluate(expr.right);

        double checkedLeft;
        double checkedRight;
        switch (expr.operaTor.type)
        {
            case TokenType.GREATER:
                CheckNumberOperands(expr.operaTor, left, right, out checkedLeft, out checkedRight);
                return checkedLeft > checkedRight;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(expr.operaTor, left, right, out checkedLeft, out checkedRight);
                return checkedLeft >= checkedRight;
            case TokenType.LESS:
                CheckNumberOperands(expr.operaTor, left, right, out checkedLeft, out checkedRight);
                return checkedLeft < checkedRight;
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(expr.operaTor, left, right, out checkedLeft, out checkedRight);
                return checkedLeft <= checkedRight;
            case TokenType.BANG_EQUAL: return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL: return IsEqual(left, right);
            case TokenType.MINUS:
                CheckNumberOperands(expr.operaTor, left, right, out checkedLeft, out checkedRight);
                return checkedLeft - checkedRight;
            case TokenType.PLUS:
                if (left is string leftString && right is string rString) return leftString + rString;
                if (left is double leftDouble && right is double rightDouble) return leftDouble + rightDouble;
                throw new RuntimeError(expr.operaTor, "Operands must be either numbers or strings");
            case TokenType.SLASH:
                CheckNumberOperands(expr.operaTor, left, right, out checkedLeft, out checkedRight);
                return checkedLeft / checkedRight;
            case TokenType.STAR:
                CheckNumberOperands(expr.operaTor, left, right, out checkedLeft, out checkedRight);
                return checkedLeft * checkedRight;
        };

        return null;
    }

    private bool IsEqual(object? a, object? b)
    {
        if (a is null && b is null) return true;
        if (a is null) return false;
        return a.Equals(b);
    }

    private string? Stringify(object? value)
    {
        return value switch
        {
            null => "nil",
            double d => d.ToString().EndsWith("0.1") ? d.ToString()[..^2] : d.ToString(),
            _ => value.ToString()
        };
    }


    public object? VisitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.expression);
    }

    public object? VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.value;
    }

    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        object? right = Evaluate(expr.right);
        double typedOperand;
        switch (expr.operaTor.type)
        {
            case TokenType.BANG:
                return !IsTruthy(right);
            case TokenType.MINUS:
                CheckNumberOperand(expr.operaTor, right, out typedOperand);
                return -typedOperand;
        }
        return null;
    }

    private void CheckNumberOperand(Token operaTor, object? operand, out double typedOperand)
    {
        if (operand is double tO)
        {
            typedOperand = tO;
            return;
        }
        throw new RuntimeError(operaTor, "Operand must be a number");
    }

    private void CheckNumberOperands(Token operaTor, object? left, object? right, out double doubleLeft, out double doubleRight)
    {
        if (left is double dL && right is double dR)
        {
            doubleLeft = dL; doubleRight = dR; return;
        };
        throw new RuntimeError(operaTor, "Operands must be a number");
    }

    private bool IsTruthy(object? value)
    {
        return value switch
        {
            null => false,
            bool => (bool)value,
            _ => true
        };
    }

    private object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    private void Execute(Stmt stmt)
    {
        stmt.Accept(this);
    }

    internal void Resolve(Expr expr, int depth)
    {
        _locals[expr] = depth;

    }

    internal void ExecuteBlock(List<Stmt> statements, Environment environment)
    {
        Environment previous = _environment;

        try
        {
            _environment = environment;

            foreach (Stmt stmt in statements)
            {
                Execute(stmt);
            }
        }
        finally
        {
            _environment = previous;
        }
    }

    public object? VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.expression);
        return null;
    }

    public object? VisitPrintStmt(Stmt.Print stmt)
    {
        object? value = Evaluate(stmt.expression);
        Console.WriteLine(Stringify(value));
        return null;
    }

    public object? VisitVarStmt(Stmt.Var stmt)
    {
        object? value = null;
        if (stmt.initializer is not null)
            value = Evaluate(stmt.initializer);

        _environment.Define(stmt.name.lexeme, value);
        return null;
    }

    public object? VisitVariableExpr(Expr.Variable expr)
    {
        return LookupVariable(expr.name, expr);
    }

    private object? LookupVariable(Token name, Expr expr)
    {
        if (_locals.TryGetValue(expr, out int distance))
        {
            return _environment.GetAt(distance, name.lexeme);
        }
        else
        {
            return globals.Get(name);
        }
    }

    public object? VisitAssignExpr(Expr.Assign expr)
    {
        object? value = Evaluate(expr.value);
        if (_locals.TryGetValue(expr, out int distance))
        {
            _environment.AssignAt(distance, expr.name, value);
        }
        else
        {
            globals.Assign(expr.name, value);
        }
        return value;
    }

    public object? VisitBlockStmt(Stmt.Block stmt)
    {
        ExecuteBlock(stmt.statements, new Environment(_environment));
        return null;
    }

    public object? VisitIfStmt(Stmt.If stmt)
    {
        if (IsTruthy(Evaluate(stmt.condition)))
        {
            Execute(stmt.thenBranch);
        }
        else if (stmt.elseBranch is not null)
        {
            Execute(stmt.elseBranch);
        }
        return null;
    }

    public object? VisitLogicalExpr(Expr.Logical expr)
    {
        object? left = Evaluate(expr.left);

        if (expr.operaTor.type is TokenType.OR)
        {
            if (IsTruthy(left))
                return left;
        }
        else
        {
            if (!IsTruthy(left))
                return left;
        }

        return Evaluate(expr.right);
    }

    public object? VisitWhileStmt(Stmt.While stmt)
    {
        while (IsTruthy(Evaluate(stmt.condition)))
        {
            Execute(stmt.body);
        }
        return null;
    }

    public object? VisitCallExpr(Expr.Call expr)
    {
        object? callee = Evaluate(expr.callee);

        List<object> args = new();
        foreach (Expr arg in expr.arguments)
        {
            if (Evaluate(arg) is object val)
                args.Add(val);
        }

        if (callee is not ILoxCallable function)
        {
            throw new RuntimeError(expr.paren, "Can only call functions and classes.");
        }

        if (args.Count != function.Arity())
        {
            throw new RuntimeError(expr.paren, $"Expected {function.Arity()} arguments bug got {args.Count}.");
        }
        return function.Call(this, args);
    }

    public object? VisitFunctionStmt(Stmt.Function stmt)
    {
        LoxFunction function = new(stmt, _environment);
        _environment.Define(stmt.name.lexeme, function);
        return null;
    }

    public object VisitReturnStmt(Stmt.Return stmt)
    {
        object? value = null;
        if (stmt.value is not null)
        {
            value = Evaluate(stmt.value);
        }
        throw new Return(value);
    }
}
