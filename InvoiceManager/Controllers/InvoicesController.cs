using AutoMapper;
using InvoiceManager.Models;
using InvoiceManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvoiceManager.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public InvoicesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            List<Invoice> lInvoices = await _context.Invoices.Include(o => o.InvoiceItems).ToListAsync();

            return View(lInvoices);
        }

        public async Task<IActionResult> Details(int id)
        {
            Invoice invoice = await _context.Invoices.Include(o => o.InvoiceItems).FirstOrDefaultAsync(o => o.Id == id);

            return invoice != null ? View(invoice) : (IActionResult)NotFound();
        }

        public IActionResult Create()
        {
            return View(new Invoice());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Invoice invoice)
        {
            if (!ModelState.IsValid)
                return View(invoice);

            invoice.CreatedDate = DateTime.Now; // this must be automatically set, not by user
            invoice.PaidDate = DateTime.MinValue; // this must be automatically set, not by user
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            Invoice invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return NotFound();

            if (invoice.GetInvoiceIsPaid())
                return ErrorInvoicePaid();

            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Invoice invoice)
        {
            if (id != invoice.Id)
                return NotFound();

            Invoice loadedInvoice = await _context.Invoices.FirstOrDefaultAsync(o => o.Id == id);
            if (loadedInvoice.GetInvoiceIsPaid())
                return ErrorInvoicePaid();

            if (!ModelState.IsValid)
                return View(invoice);

            try
            {
                invoice.CreatedDate = loadedInvoice.CreatedDate; // this must be retained (is not allowed to edit it)
                invoice.PaidDate = loadedInvoice.PaidDate; // this must be retained (is not allowed to edit it)
                _mapper.Map(invoice, loadedInvoice);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await InvoiceExistsAsync(invoice.Id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Invoice invoice = await _context.Invoices.Include(o => o.InvoiceItems).FirstOrDefaultAsync(o => o.Id == id);

            if (invoice == null)
                return NotFound();

            if (invoice.GetInvoiceIsPaid())
                return ErrorInvoicePaid();

            return View(invoice);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Invoice invoice = await _context.Invoices.FindAsync(id);
            if (invoice.GetInvoiceIsPaid())
                return ErrorInvoicePaid();

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
        private async Task<bool> InvoiceExistsAsync(int id)
        {
            return await _context.Invoices.AnyAsync(o => o.Id == id);
        }
    }
}
