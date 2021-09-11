namespace CsLoxInterpreter.Utilities
{
    public static class CheapExtensions
    {
        public static T Peek<T>(this List<T> @this) => @this[@this.Count - 1];

    }
}
