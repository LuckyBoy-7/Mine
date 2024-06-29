namespace Lucky.Extensions
{
    public static class FloatExtensions
    {
        public static float GetDecimal(this float orig)
        {
            return orig - (int)orig;
        }
    }
}