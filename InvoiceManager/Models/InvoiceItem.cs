using System;
using System.ComponentModel.DataAnnotations;

namespace InvoiceManager.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(512)]
        [Display(Name = "Invoice item subject")]
        public string InvoiceItemSubject { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Price { get; set; }

        [Required]
        public virtual Invoice Invoice { get; set; }
    }
}
