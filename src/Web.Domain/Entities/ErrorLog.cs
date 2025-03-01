namespace Web.Domain.Entities
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime DateOccurred { get; set; }
        public string Severity { get; set; } // Ex.: "Error", "Warning", "Info"
    }
}