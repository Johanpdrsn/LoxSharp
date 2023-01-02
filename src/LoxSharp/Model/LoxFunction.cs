using LoxSharp.Error;
using LoxSharp.Interface;

namespace LoxSharp.Model;

internal class LoxFunction : ILoxCallable
{
    private readonly Stmt.Function _declaration;
    private readonly Environment _closure;
    private readonly bool _isInitializer;

    public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer)
    {
        _isInitializer = isInitializer;
        _declaration = declaration;
        _closure = closure;
    }

    internal LoxFunction Bind(LoxInstance instance)
    {
        Environment environment = new(_closure);
        environment.Define("this", instance);
        return new LoxFunction(_declaration, environment, _isInitializer);
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
            if (_isInitializer)
                return _closure.GetAt(0, "this");

            return r.value;
        }
        if (_isInitializer)
            return _closure.GetAt(0, "this");

        return null;
    }

    public override string ToString() => $"<fn {_declaration.name.lexeme}>";
}
