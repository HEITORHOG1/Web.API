namespace Web.Domain.DTOs.MercadoPago
{
    public class TransactionReportDto
    {
        public string ReportId { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalAmount { get; set; }
        public List<TransactionDetail> Transactions { get; set; }

        public class TransactionDetail
        {
            public string Id { get; set; }
            public string Status { get; set; }
            public string ExternalReference { get; set; }
            public decimal Amount { get; set; }
            public DateTime DateCreated { get; set; }
            public string PaymentMethod { get; set; }
            public string PaymentType { get; set; }
        }
    }
}
