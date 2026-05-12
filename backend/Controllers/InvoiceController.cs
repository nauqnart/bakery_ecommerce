using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StitchArtisan.Backend.Data;

namespace StitchArtisan.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InvoiceController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/invoice
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Order)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
            return Ok(new { Success = true, Data = invoices });
        }

        // GET api/invoice/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Order).ThenInclude(o => o!.Items).ThenInclude(item => item.Variant)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
                return NotFound(new { Success = false, Message = "Invoice not found" });

            return Ok(new { Success = true, Data = invoice });
        }

        // GET api/invoice/order/{orderId}
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Order)
                .FirstOrDefaultAsync(i => i.OrderId == orderId);

            if (invoice == null)
                return NotFound(new { Success = false, Message = "Invoice not found for this order" });

            return Ok(new { Success = true, Data = invoice });
        }

        // PUT api/invoice/{id}/pay — đánh dấu đã thanh toán
        [HttpPut("{id}/pay")]
        public async Task<IActionResult> MarkAsPaid(int id, [FromBody] string paymentMethod)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return NotFound(new { Success = false, Message = "Invoice not found" });

            invoice.PaymentStatus = "paid";
            invoice.PaymentMethod = paymentMethod;
            invoice.PaidAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Invoice marked as paid", Data = invoice });
        }
    }
}
