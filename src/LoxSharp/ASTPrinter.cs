using LoxSharp.Model;
using System.Text;

namespace LoxSharp;

public class ASTPrinter : Expr.Visitor<string>
{
    public string Print(Expr expr) => expr.Accept(this);

    string Expr.Visitor<string>.VisitBinaryExpr(Expr.Binary expr) => Parenthesize(expr.operaTor.lexeme, expr.left, expr.right);


    string Expr.Visitor<string>.VisitGroupingExpr(Expr.Grouping expr) => Parenthesize("group", expr.expression);


    string Expr.Visitor<string>.VisitLiteralExpr(Expr.Literal expr) => expr.value != null ? expr.value.ToString()! : "nil";


    string Expr.Visitor<string>.VisitUnaryExpr(Expr.Unary expr) => Parenthesize(expr.operaTor.lexeme, expr.right);

    private string Parenthesize(string name, params Expr[] exprs)
    {
        StringBuilder sb = new();

        sb.Append('(').Append(name);

        foreach (Expr expr in exprs)
        {
            sb.Append(" ");
            sb.Append(expr.Accept(this));
        }
        sb.Append(')');

        return sb.ToString();
    }

    public string VisitAssignExpr(Expr.Assign expr)
    {
        throw new NotImplementedException();
    }

    public string VisitVariableExpr(Expr.Variable expr)
    {
        throw new NotImplementedException();
    }

    public string VisitLogicalExpr(Expr.Logical expr)
    {
        throw new NotImplementedException();
    }

    public string VisitCallExpr(Expr.Call expr)
    {
        throw new NotImplementedException();
    }

    public string VisitGetExpr(Expr.Get expr)
    {
        throw new NotImplementedException();
    }

    public string VisitSetExpr(Expr.Set expr)
    {
        throw new NotImplementedException();
    }

    public string VisitThisExpr(Expr.This expr)
    {
        throw new NotImplementedException();
    }

    public string VisitSuperExpr(Expr.Super expr)
    {
        throw new NotImplementedException();
    }
}
