namespace LoxSharp;

public class Interpreter : Expr.Visitor<object>
{
    public void Interpret(Expr expression)
    {
        try
        {
            object value = Evaluate(expression);
            Console.WriteLine(Stringify(value));
        }
        catch (RuntimeError ex)
        {
            LoxSharp.RuntimeError(ex);
        }
    }

    public object VisitBinaryExpr(Expr.Binary expr)
    {
        object left = Evaluate(expr.left);
        object right = Evaluate(expr.right);

        switch (expr.operaTor.type)
        {
            case TokenType.GREATER:
                CheckNumberOperands(expr.operaTor, left, right);
                return (double)left > (double)right;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(expr.operaTor, left, right);
                return (double)left >= (double)right;
            case TokenType.LESS:
                CheckNumberOperands(expr.operaTor, left, right);
                return (double)left < (double)right;
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(expr.operaTor, left, right);
                return (double)left <= (double)right;
            case TokenType.BANG_EQUAL: return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL: return IsEqual(left, right);
            case TokenType.MINUS:
                CheckNumberOperands(expr.operaTor, left, right);
                return (double)left - (double)right;
            case TokenType.PLUS:
                if (left is string leftString && right is string rString) return leftString + rString;
                if (left is double leftDouble && right is double rightDouble) return leftDouble + rightDouble;
                throw new RuntimeError(expr.operaTor, "Operands must be either numbers or strings");
            case TokenType.SLASH:
                CheckNumberOperands(expr.operaTor, left, right);
                return (double)left / (double)right;
            case TokenType.STAR:
                CheckNumberOperands(expr.operaTor, left, right);
                return (double)left * (double)right;
        };

        return null;
    }

    private bool IsEqual(object a, object b)
    {
        if (a is null && b is null) return true;
        if (a is null) return false;
        return a.Equals(b);
    }

    private string Stringify(object value)
    {
        return value switch
        {
            null => "nil",
            double d => value.ToString().EndsWith("0.1") ? value.ToString()[..^2] : value.ToString(),
            _ => value.ToString()
        };
    }


    public object VisitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.expression);
    }

    public object VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.value;
    }

    public object VisitUnaryExpr(Expr.Unary expr)
    {
        object right = Evaluate(expr.right);

        switch (expr.operaTor.type)
        {
            case TokenType.BANG:
                return !IsTruthy(right);
            case TokenType.MINUS:
                CheckNumberOperand(expr.operaTor, right);
                return -(double)right;
        }
        return null;
    }

    private void CheckNumberOperand(Token operaTor, object operand)
    {
        if (operand is double) return;
        throw new RuntimeError(operaTor, "Operand must be a number");
    }

    private void CheckNumberOperands(Token operaTor, object left, object right)
    {
        if (left is double && right is double) return;
        throw new RuntimeError(operaTor, "Operands must be a number");
    }

    private bool IsTruthy(object value)
    {
        return value switch
        {
            null => false,
            bool => (bool)value,
            _ => true
        };
    }

    private object Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }
}
