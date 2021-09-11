namespace CsLoxInterpreter.Calling.NativeFunctions
{
    internal class ClockLoxCallable : ILoxCallable
    {
        public ClockLoxCallable() { }

        public int Arity() => 0;

        public object Call(Interpreter interpreter, List<object> arguments)
        {

            return ((Double)(System.DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond)) / 1000D;
        }

        public override string ToString()
        {
            return "<Native fn>";
        }
    }
}
