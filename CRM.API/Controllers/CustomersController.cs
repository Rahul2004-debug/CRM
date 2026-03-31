using CRM.API.Data;
using CRM.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CRM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var claim = User.Claims.FirstOrDefault(c =>
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (claim == null || string.IsNullOrEmpty(claim.Value))
                return 0;

            return int.Parse(claim.Value);
        }

        private string GetRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized("Invalid user");

            var role = GetRole();

            var query = _context.Customers.AsQueryable();

            if (role != "Admin")
            {
                query = query.Where(c => c.UserId == userId);
            }

            var customers = await query.ToListAsync();

            return Ok(customers);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCustomers(string queryText)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized("Invalid user");

            var role = GetRole();

            var query = _context.Customers.AsQueryable();

            if (role != "Admin")
            {
                query = query.Where(c => c.UserId == userId);
            }

            var customers = await query
                .Where(c =>
                    c.Name.Contains(queryText) ||
                    c.Email.Contains(queryText) ||
                    c.Company.Contains(queryText))
                .ToListAsync();

            return Ok(customers);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterCustomers(string status)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized("Invalid user");

            var role = GetRole();

            var query = _context.Customers.Where(c => c.Status == status);

            if (role != "Admin")
            {
                query = query.Where(c => c.UserId == userId);
            }

            var customers = await query.ToListAsync();

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized("Invalid user");

            var role = GetRole();

            Customer customer;

            if (role == "Admin")
            {
                customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            }
            else
            {
                customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            }

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.Name) ||
                string.IsNullOrWhiteSpace(customer.Email) ||
                string.IsNullOrWhiteSpace(customer.Phone))
            {
                return BadRequest("Name, Email and Phone are required");
            }

            var userId = GetUserId();
            if (userId == 0) return Unauthorized("Invalid user");

            bool exists = await _context.Customers
                .AnyAsync(c =>
                    c.UserId == userId &&
                    (c.Email == customer.Email || c.Phone == customer.Phone));

            if (exists)
                return BadRequest("Duplicate Email or Phone not allowed");

            customer.UserId = userId;

            _context.Notifications.Add(new Notification
            {
                Message = "New customer added: " + customer.Name,
                CreatedAt = DateTime.UtcNow
            });

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok(customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customer updatedCustomer)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized("Invalid user");

            var role = GetRole();

            var customer = role == "Admin"
                ? await _context.Customers.FirstOrDefaultAsync(c => c.Id == id)
                : await _context.Customers.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (customer == null)
                return NotFound();

            customer.Name = updatedCustomer.Name;
            customer.Email = updatedCustomer.Email;
            customer.Phone = updatedCustomer.Phone;
            customer.Company = updatedCustomer.Company;
            customer.Address = updatedCustomer.Address;
            customer.Status = updatedCustomer.Status;

            await _context.SaveChangesAsync();

            return Ok(customer);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok("Customer deleted successfully");
        }
    }
}