namespace SequenceGenerator.Exceptions
{
    public class SequenceTemplateNotFoundException : Exception
    {
        public SequenceTemplateNotFoundException()
            : base("Sequence template definition not found")
        {
        }
    }
}
