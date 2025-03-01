namespace Web.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> Errors { get; }

        public ValidationException(string message, List<string> errors)
            : base(message)
        {
            Errors = errors;
        }

        public override string ToString()
        {
            return $"{Message}\nDetalhes: {string.Join("\n", Errors)}";
        }
    }
}