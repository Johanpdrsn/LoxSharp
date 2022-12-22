﻿using LoxSharp.Error;

namespace LoxSharp.Model;

public class Environment
{
    private Environment? _enclosing;
    private readonly Dictionary<string, object?> _values = new();

    internal Environment()
    {
        _enclosing = null;
    }

    internal Environment(Environment environment)
    {
        _enclosing = environment;
    }

    internal void Define(string name, object? value)
    {
        _values[name] = value;
    }

    internal object? GetAt(int distance, string name)
    {
        return Ancestor(distance)!._values[name];
    }

    private Environment? Ancestor(int distance)
    {
        Environment? environment = this;
        for (int i = 0; i < distance; i++)
        {
            if (environment is not null)
                environment = environment._enclosing;
        }
        return environment;
    }

    internal object Get(Token name)
    {
        var exist = _values.TryGetValue(name.lexeme, out object? value);

        if (exist)
            return value!;

        if (_enclosing is not null)
            return _enclosing.Get(name);

        throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
    }

    internal void Assign(Token name, object? value)
    {
        if (_values.ContainsKey(name.lexeme))
        {
            _values[name.lexeme] = value;
            return;
        }

        if (_enclosing is not null)
        {
            _enclosing.Assign(name, value);
            return;
        }
        throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'");
    }

    internal void AssignAt(int distance, Token name, object? value)
    {
        Ancestor(distance)!._values[name.lexeme] = value;
    }

}