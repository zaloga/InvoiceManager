using AutoMapper;
using InvoiceManager.Models;
using InvoiceManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public InvoicesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceApiGetDTO>>> GetInvoices(bool bOnlyUnpaid)
        {
            if (!SecretKeyIsCorrect())
                return ErrorSecretKeyNotCorrect();

            IQueryable<Invoice> query = _context.Invoices;
            if (bOnlyUnpaid)
                query = query.Where(o => o.PaidDate == DateTime.MinValue);

            List<InvoiceApiGetDTO> lInvoicesVM = new List<InvoiceApiGetDTO>();

            List<Invoice> lInvoices = await query.Include(o => o.InvoiceItems).ToListAsync();
            foreach (Invoice invoice in lInvoices)
            {
                InvoiceApiGetDTO invoiceVM = new InvoiceApiGetDTO();
                _mapper.Map(invoice, invoiceVM);
                lInvoicesVM.Add(invoiceVM);
            }

            return lInvoicesVM;
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> EditInvoice(int id, InvoiceApiEditDTO invoice)
        {
            if (!SecretKeyIsCorrect())
                return ErrorSecretKeyNotCorrect();

            Invoice loadedInvoice = await _context.Invoices.FirstOrDefaultAsync(o => o.Id == id);
            if (loadedInvoice.GetInvoiceIsPaid())
                return ErrorInvoicePaid();

            try
            {
                _mapper.Map(invoice, loadedInvoice);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await InvoiceExistsAsync(id))
                    return NotFound();
                throw;
            }

            return Ok(new { message = "Invoice was saved" });
        }

        [HttpPost("Pay/{id:int}")]
        public async Task<IActionResult> PayInvoice(int id)
        {
            if (!SecretKeyIsCorrect())
                return ErrorSecretKeyNotCorrect();

            Invoice invoice = await _context.Invoices.Include(o => o.InvoiceItems).FirstOrDefaultAsync(o => o.Id == id);
            if (invoice == null)
                return NotFound();

            if (invoice.GetTotalPrice() == 0)
                return BadRequest(new { message = "This invoice has no item to pay" });

            if (invoice.GetInvoiceIsPaid())
                return ErrorInvoicePaid();

            invoice.PaidDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Invoice was paid" });
        }

        /// <summary>
        /// helper method
        /// </summary>
        private bool SecretKeyIsCorrect()
        {
            StringValues stringValues;
            HttpContext.Request.Headers.TryGetValue(Constants.ApiSecretKeyName, out stringValues);

            return stringValues.Count > 0 && Constants.ApiSecretKeyValue == stringValues[0];
        }

        /// <summary>
        /// helper method
        /// </summary>
        private ObjectResult ErrorSecretKeyNotCorrect()
        {
            return Unauthorized(new { message = "Your secret key is not correct" });
        }

        /// <summary>
        /// helper method
        /// </summary>
        private ObjectResult ErrorInvoicePaid()
        {
            return BadRequest(new { message = Constants.ErrorMessageInvoicePaid });
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
