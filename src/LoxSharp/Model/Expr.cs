namespace LoxSharp.Model;

public abstract class Expr
{
    public interface Visitor<T>
    {
        public T VisitAssignExpr(Assign expr);
        public T VisitBinaryExpr(Binary expr);
        public T VisitCallExpr(Call expr);
        public T VisitGetExpr(Get expr);
        public T VisitGroupingExpr(Grouping expr);
        public T VisitLiteralExpr(Literal expr);
        public T VisitLogicalExpr(Logical expr);
        public T VisitSetExpr(Set expr);
        public T VisitThisExpr(This expr);
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
    public class Call : Expr
    {
        public Call(Expr callee, Token paren, List<Expr> arguments)
        {
            this.callee = callee;
            this.paren = paren;
            this.arguments = arguments;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitCallExpr(this);
        }

        public readonly Expr callee;
        public readonly Token paren;
        public readonly List<Expr> arguments;
    }
    public class Get : Expr
    {
        public Get(Expr obj, Token name)
        {
            this.obj = obj;
            this.name = name;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitGetExpr(this);
        }

        public readonly Expr obj;
        public readonly Token name;
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
        public Literal(object? value)
        {
            this.value = value;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }

        public readonly object? value;
    }
    public class Logical : Expr
    {
        public Logical(Expr left, Token operaTor, Expr right)
        {
            this.left = left;
            this.operaTor = operaTor;
            this.right = right;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitLogicalExpr(this);
        }

        public readonly Expr left;
        public readonly Token operaTor;
        public readonly Expr right;
    }
    public class Set : Expr
    {
        public Set(Expr obj, Token name, Expr value)
        {
            this.obj = obj;
            this.name = name;
            this.value = value;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitSetExpr(this);
        }

        public readonly Expr obj;
        public readonly Token name;
        public readonly Expr value;
    }
    public class This : Expr
    {
        public This(Token keyword)
        {
            this.keyword = keyword;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitThisExpr(this);
        }

        public readonly Token keyword;
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
