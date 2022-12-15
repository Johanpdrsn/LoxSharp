namespace LoxSharp;

public abstract class Expr
{
    public interface Visitor<T>
    {
        public T VisitAssignExpr(Assign expr);
        public T VisitBinaryExpr(Binary expr);
        public T VisitGroupingExpr(Grouping expr);
        public T VisitLiteralExpr(Literal expr);
        public T VisitUnaryExpr(Unary expr);
        public T VisitVariableExpr(Variable expr);
    }
    public class Assign : Expr
    {
        public Assign(Token name, Expr value)
        {
            this.name = name;
            this.value = value;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitAssignExpr(this);
        }

        public readonly Token name;
        public readonly Expr value;
    }
    public class Binary : Expr
    {
        public Binary(Expr left, Token operaTor, Expr right)
        {
            this.left = left;
            this.operaTor = operaTor;
            this.right = right;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }

        public readonly Expr left;
        public readonly Token operaTor;
        public readonly Expr right;
    }
    public class Grouping : Expr
    {
        public Grouping(Expr expression)
        {
            this.expression = expression;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }

        public readonly Expr expression;
    }
    public class Literal : Expr
    {
        public Literal(Object value)
        {
            this.value = value;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }

        public readonly Object value;
    }
    public class Unary : Expr
    {
        public Unary(Token operaTor, Expr right)
        {
            this.operaTor = operaTor;
            this.right = right;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }

        public readonly Token operaTor;
        public readonly Expr right;
    }
    public class Variable : Expr
    {
        public Variable(Token name)
        {
            this.name = name;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitVariableExpr(this);
        }

        public readonly Token name;
    }

    public abstract T Accept<T>(Visitor<T> visitor);
}
