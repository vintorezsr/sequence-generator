namespace SequenceGenerator.Exceptions
{
    public class InvalidSequenceNumberException : Exception
    {
        public InvalidSequenceNumberException()
            : base("Invalid sequence number")
        {
        }
    }
}
