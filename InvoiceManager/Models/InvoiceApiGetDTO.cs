using System;

namespace InvoiceManager.Models
{
    public class InvoiceApiGetDTO
    {
        public int Id { get; set; }

        public string InvoiceNumber { get; set; }

        public string Supplier { get; set; }

        public string Customer { get; set; }

        public string InvoiceSubject { get; set; }

        public string PayMethod { get; set; }

        public string BankAccountNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime PaidDate { get; set; }

        public DateTime DueDate { get; set; }

        public int InvoiceItemsCount { get; set; }

        public int TotalPrice { get; set; }
    }
}
