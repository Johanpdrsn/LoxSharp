using LoxSharp.Interface;

namespace LoxSharp.Model;

internal class LoxClass : ILoxCallable
{
    internal readonly string name;
    private readonly Dictionary<string, LoxFunction> _methods;

    public LoxClass(string name, Dictionary<string, LoxFunction> methods)
    {
        this.name = name;
        _methods = methods;
    }

    public int Arity()
    {
        LoxFunction? initializer = FindMethod("init");
        return initializer is null ? 0 : initializer.Arity();
    }

    public object? Call(Interpreter interpreter, List<object> args)
    {
        LoxInstance instance = new(this);
        LoxFunction? initializer = FindMethod("init");

        initializer?.Bind(instance).Call(interpreter, args);

        return instance;
    }

    public override string ToString() => name;

    internal LoxFunction? FindMethod(string name)
    {
        if (_methods.TryGetValue(name, out var method))
            return method;

        return null;
    }
}
