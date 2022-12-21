using LoxSharp.Error;
using LoxSharp.Interface;

namespace LoxSharp.Model;

internal class LoxFunction : ILoxCallable
{
    private readonly Stmt.Function _declaration;
    private readonly Environment _closure;

    public LoxFunction(Stmt.Function declaration, Environment closure)
    {
        _declaration = declaration;
        _closure = closure;
    }

    public int Arity() => _declaration.parameters.Count;

    public object? Call(Interpreter interpreter, List<object> args)
    {
        Environment environment = new(_closure);
        for (int i = 0; i < _declaration.parameters.Count; i++)
        {
            environment.Define(_declaration.parameters[i].lexeme, args[i]);
        }
        try
        {
            interpreter.ExecuteBlock(_declaration.body, environment);
        }
        catch (Return r)
        {
            return r.value;
        }
        return null;
    }

    public override string ToString() => $"<fn {_declaration.name.lexeme}>";
}
