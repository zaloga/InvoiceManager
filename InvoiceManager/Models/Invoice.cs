using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace InvoiceManager.Models
{
    public class Invoice
    {
        private const int DueDateOffset = 30;

        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        [Display(Name = "Invoice number")]
        public string InvoiceNumber { get; set; } = string.Format("{0:yyyyMMddhhmmss}", DateTime.Now);

        [Required]
        [MaxLength(256)]
        public string Supplier { get; set; }

        [Required]
        [MaxLength(256)]
        public string Customer { get; set; }

        [Required]
        [MaxLength(512)]
        [Display(Name = "Invoice subject")]
        public string InvoiceSubject { get; set; } = "Services and products";

        [Required]
        [MaxLength(32)]
        [Display(Name = "Pay method")]
        public string PayMethod { get; set; } = "bank transfer";

        [Required]
        [MaxLength(64)]
        [Display(Name = "Bank account number")]
        public string BankAccountNumber { get; set; }

        [Required]
        [Display(Name = "Created")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Paid date")]
        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime PaidDate { get; set; } = DateTime.MinValue;

        [Required]
        [Display(Name = "Due date")]
        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(DueDateOffset);

        [Display(Name = "Invoice items")]
        public virtual List<InvoiceItem> InvoiceItems { get; set; }

        public int GetInvoiceItemsCount() => InvoiceItems != null ? InvoiceItems.Count() : 0;

        public int GetTotalPrice() => InvoiceItems != null ? InvoiceItems.Sum(o => o.Price) : 0;

        public bool GetInvoiceIsPaid() => this.PaidDate > DateTime.MinValue;
    }
}
