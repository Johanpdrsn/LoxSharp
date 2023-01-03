namespace LoxSharp.Model;

public abstract class Stmt
{
    public interface Visitor<T>
    {
        public T VisitBlockStmt(Block stmt);
        public T VisitClassStmt(Class stmt);
        public T VisitExpressionStmt(Expression stmt);
        public T VisitFunctionStmt(Function stmt);
        public T VisitIfStmt(If stmt);
        public T VisitPrintStmt(Print stmt);
        public T VisitReturnStmt(Return stmt);
        public T VisitVarStmt(Var stmt);
        public T VisitWhileStmt(While stmt);
    }
    public class Block : Stmt
    {
        public Block(List<Stmt> statements)
        {
            this.statements = statements;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }

        public readonly List<Stmt> statements;
    }
    public class Class : Stmt
    {
        public Class(Token name, Expr.Variable? superClass, List<Stmt.Function> methods)
        {
            this.name = name;
            this.superClass = superClass;
            this.methods = methods;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitClassStmt(this);
        }

        public readonly Token name;
        public readonly Expr.Variable? superClass;
        public readonly List<Stmt.Function> methods;
    }
    public class Expression : Stmt
    {
        public Expression(Expr expression)
        {
            this.expression = expression;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }

        public readonly Expr expression;
    }
    public class Function : Stmt
    {
        public Function(Token name, List<Token> parameters, List<Stmt> body)
        {
            this.name = name;
            this.parameters = parameters;
            this.body = body;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitFunctionStmt(this);
        }

        public readonly Token name;
        public readonly List<Token> parameters;
        public readonly List<Stmt> body;
    }
    public class If : Stmt
    {
        public If(Expr condition, Stmt thenBranch, Stmt? elseBranch)
        {
            this.condition = condition;
            this.thenBranch = thenBranch;
            this.elseBranch = elseBranch;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitIfStmt(this);
        }

        public readonly Expr condition;
        public readonly Stmt thenBranch;
        public readonly Stmt? elseBranch;
    }
    public class Print : Stmt
    {
        public Print(Expr expression)
        {
            this.expression = expression;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }

        public readonly Expr expression;
    }
    public class Return : Stmt
    {
        public Return(Token keyword, Expr? value)
        {
            this.keyword = keyword;
            this.value = value;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitReturnStmt(this);
        }

        public readonly Token keyword;
        public readonly Expr? value;
    }
    public class Var : Stmt
    {
        public Var(Token name, Expr? initializer)
        {
            this.name = name;
            this.initializer = initializer;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitVarStmt(this);
        }

        public readonly Token name;
        public readonly Expr? initializer;
    }
    public class While : Stmt
    {
        public While(Expr condition, Stmt body)
        {
            this.condition = condition;
            this.body = body;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitWhileStmt(this);
        }

        public readonly Expr condition;
        public readonly Stmt body;
    }

    public abstract T Accept<T>(Visitor<T> visitor);
}
