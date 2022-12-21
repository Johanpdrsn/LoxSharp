namespace LoxSharp.Interface;

internal interface ILoxCallable
{
    object? Call(Interpreter interpreter, List<object> args);
    int Arity();
}
