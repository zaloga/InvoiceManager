using AutoMapper;
using InvoiceManager.Models;
using InvoiceManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InvoiceManager.Controllers
{
    public class InvoiceItemsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public InvoiceItemsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index(int invoiceId)
        { 
            return RedirectToAction(
                nameof(InvoicesController.Details), 
                nameof(InvoicesController).Replace("Controller", string.Empty), 
                new { id = invoiceId });
        }

        public async Task<IActionResult> Details(int id)
        {
            InvoiceItem invoiceItem = await _context.InvoiceItems.Include(o => o.Invoice).FirstOrDefaultAsync(o => o.Id == id);
            if (invoiceItem == null)
                return NotFound();

            InvoiceItemViewModel invoiceItemVM = _mapper.Map<InvoiceItemViewModel>(invoiceItem);

            return View(invoiceItemVM);
        }

        public async Task<IActionResult> Create(int invoiceId)
        {
            Invoice invoice = await _context.Invoices.FirstOrDefaultAsync(o => o.Id == invoiceId);
            if (invoice.GetInvoiceIsPaid())
                return ErrorInvoicePaid();

            return View(new InvoiceItemViewModel() { InvoiceId = invoiceId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceItemViewModel invoiceItemVM)
        {
            if (!ModelState.IsValid)
                return View(invoiceItemVM);

            Invoice invoice = await _context.Invoices.FirstOrDefaultAsync(o => o.Id == invoiceItemVM.InvoiceId);
            if (invoice.GetInvoiceIsPaid())
                return ErrorInvoicePaid();

            InvoiceItem invoiceItem = _mapper.Map<InvoiceItem>(invoiceItemVM);
            invoiceItem.Invoice = invoice;
            _context.InvoiceItems.Add(invoiceItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(HomeController.Index), new { invoiceId = invoiceItemVM.InvoiceId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            InvoiceItem invoiceItem = await _context.InvoiceItems.Include(o => o.Invoice).FirstOrDefaultAsync(o => o.Id == id);
            if (invoiceItem == null)
                return NotFound();

            if (invoiceItem.Invoice.GetInvoiceIsPaid())
                return ErrorInvoicePaid();

            InvoiceItemViewModel invoiceItemVM = _mapper.Map<InvoiceItemViewModel>(invoiceItem);

            return View(invoiceItemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InvoiceItemViewModel invoiceItemVM)
        {
            if (id != invoiceItemVM.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(invoiceItemVM);

            Invoice invoice = await _context.Invoices.FirstOrDefaultAsync(o => o.Id == invoiceItemVM.InvoiceId);
            if (invoice.GetInvoiceIsPaid())
                return ErrorInvoicePaid();

            try
            {
                InvoiceItem invoiceItem = _mapper.Map<InvoiceItem>(invoiceItemVM);
                invoiceItem.Invoice = invoice;
                _context.InvoiceItems.Update(invoiceItem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await InvoiceItemExistsAsync(invoiceItemVM.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(HomeController.Index), new { invoiceId = invoiceItemVM.InvoiceId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            InvoiceItem invoiceItem = await _context.InvoiceItems.Include(o => o.Invoice).FirstOrDefaultAsync(o => o.Id == id);
            if (invoiceItem == null)
                return NotFound();

            if (invoiceItem.Invoice.GetInvoiceIsPaid())
                return ErrorInvoicePaid();

            InvoiceItemViewModel invoiceItemVM = _mapper.Map<InvoiceItemViewModel>(invoiceItem);

            return View(invoiceItemVM);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            InvoiceItem invoiceItem = await _context.InvoiceItems.Include(o => o.Invoice).FirstOrDefaultAsync(o => o.Id == id);
            if (invoiceItem.Invoice.GetInvoiceIsPaid())
                return ErrorInvoicePaid();

            _context.InvoiceItems.Remove(invoiceItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(HomeController.Index), new { invoiceId = invoiceItem.Invoice.Id });
        }

        /// <summary>
        /// helper method
        /// </summary>
        private IActionResult ErrorInvoicePaid()
        { 
            return RedirectToAction(
                nameof(HomeController.Error), 
                nameof(HomeController).Replace("Controller", string.Empty), 
                new { ErrorMessage = Constants.ErrorMessageInvoicePaid });
        }

        /// <summary>
        /// helper method
        /// </summary>
        private async Task<bool> InvoiceItemExistsAsync(int id)
        {
            return await _context.InvoiceItems.AnyAsync(o => o.Id == id);
        }
    }
}
