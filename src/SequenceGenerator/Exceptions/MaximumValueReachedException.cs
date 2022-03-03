namespace SequenceGenerator.Exceptions
{
    public class MaximumValueReachedException : Exception
    {
        public MaximumValueReachedException()
            : base("Maximum value has been reached")
        {
        }
    }
}
