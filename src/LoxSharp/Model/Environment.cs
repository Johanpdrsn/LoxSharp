using LoxSharp.Error;

namespace LoxSharp.Model;

public class Environment
{
    internal readonly Environment? enclosing;
    private readonly Dictionary<string, object?> _values = new();

    internal Environment()
    {
        enclosing = null;
    }

    internal Environment(Environment environment)
    {
        enclosing = environment;
    }

    internal void Define(string name, object? value)
    {
        _values[name] = value;
    }

    internal object? GetAt(int distance, string name)
    {
        return Ancestor(distance)!._values.TryGetValue(name, out object? val) ? val : null;
    }

    private Environment? Ancestor(int distance)
    {
        Environment? environment = this;
        // this should maybe be 0
        for (int i = 0; i < distance; i++)
        {
            if (environment is not null)
                environment = environment.enclosing;
        }
        return environment;
    }

    internal object Get(Token name)
    {
        var exist = _values.TryGetValue(name.lexeme, out object? value);

        if (exist)
            return value!;

        if (enclosing is not null)
            return enclosing.Get(name);

        throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
    }

    internal void Assign(Token name, object? value)
    {
        if (_values.ContainsKey(name.lexeme))
        {
            _values[name.lexeme] = value;
            return;
        }

        if (enclosing is not null)
        {
            enclosing.Assign(name, value);
            return;
        }
        throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'");
    }

    internal void AssignAt(int distance, Token name, object? value)
    {
        Ancestor(distance)!._values[name.lexeme] = value;
    }

}
