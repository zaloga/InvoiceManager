using System.Collections.Generic;

namespace InvoiceManager.Models
{
    public class InvoiceItemsViewModel
    {
        public int InvoiceId { get; set; }

        public List<InvoiceItem> InvoiceItems { get; set; }
    }
}
