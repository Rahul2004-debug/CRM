using CRM.API.Data;
using CRM.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CRM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var claim = User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier);

            if (claim == null || string.IsNullOrEmpty(claim.Value))
                return 0;

            return int.Parse(claim.Value);
        }

        private string GetRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }

        // ✅ GET CONTACTS (ONLY SALES REP DATA)
        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            var userId = GetUserId();
            var role = GetRole();

            if (userId == 0)
                return Unauthorized("Invalid user");

            var query = _context.Contacts.AsQueryable();

            if (role != "Admin")
            {
                query = query.Where(c => c.UserId == userId);
            }

            var contacts = await query.ToListAsync();

            return Ok(contacts);
        }

        // ✅ ADD CONTACT WITH CUSTOMER VALIDATION
        [HttpPost]
        public async Task<IActionResult> AddContact(Contact contact)
        {
            if (contact == null)
                return BadRequest("Invalid data");

            var userId = GetUserId();
            if (userId == 0)
                return Unauthorized("Invalid user");

            // ✅ CHECK CUSTOMER EXISTS
            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == contact.CustomerId);

            if (!customerExists)
                return BadRequest("Customer ID not found");

            contact.UserId = userId;

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return Ok(contact);
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var userId = GetUserId();
            var role = GetRole();

            var contact = role == "Admin"
                ? await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id)
                : await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (contact == null)
                return NotFound();

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}