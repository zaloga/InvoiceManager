using System;
using System.ComponentModel.DataAnnotations;

namespace InvoiceManager.Models
{
    public class InvoiceApiEditDTO
    {
        [MaxLength(32)]
        [Display(Name = "Invoice number")]
        public string InvoiceNumber { get; set; }

        [MaxLength(256)]
        public string Supplier { get; set; }

        [MaxLength(256)]
        public string Customer { get; set; }

        [MaxLength(512)]
        [Display(Name = "Invoice subject")]
        public string InvoiceSubject { get; set; }

        [MaxLength(32)]
        [Display(Name = "Pay method")]
        public string PayMethod { get; set; }

        [MaxLength(64)]
        [Display(Name = "Bank account number")]
        public string BankAccountNumber { get; set; }

        [Display(Name = "Due date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? DueDate { get; set; }
    }
}
