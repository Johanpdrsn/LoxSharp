using LoxSharp.Interface;

namespace LoxSharp.NativeFunctions;

internal class Clock : ILoxCallable
{
    public int Arity() => 0;

    public object Call(Interpreter interpreter, List<object> args) => DateTime.Now.Second;

    public override string ToString() => "<native fn>";
}
