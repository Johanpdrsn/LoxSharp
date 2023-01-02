using LoxSharp.Error;

namespace LoxSharp.Model;

internal class LoxInstance
{
    private LoxClass _class;
    private readonly Dictionary<string, object?> _fields = new();

    internal LoxInstance(LoxClass klass)
    {
        _class = klass;
    }

    public override string ToString() => $"{_class.name} instance";

    internal object? Get(Token name)
    {
        if (_fields.TryGetValue(name.lexeme, out object? val))
        {
            return val;
        }

        LoxFunction? method = _class.FindMethod(name.lexeme);
        if (method is not null)
            return method.Bind(this);

        throw new RuntimeError(name, $"Undefined property '{name.lexeme}'.");
    }

    internal void Set(Token name, object? value)
    {
        _fields.Add(name.lexeme, value);
    }
}
