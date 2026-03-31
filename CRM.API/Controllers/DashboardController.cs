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
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
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

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var userId = GetUserId();
            var role = GetRole();

            IQueryable<Customer> customerQuery = _context.Customers;

            if (role != "Admin")
            {
                customerQuery = customerQuery.Where(c => c.UserId == userId);
            }

            var totalCustomers = await customerQuery.CountAsync();

            var totalContacts = role == "Admin"
                ? await _context.Contacts.CountAsync()
                : await _context.Contacts.Where(c => c.UserId == userId).CountAsync();

            var totalNotes = await _context.Notes.CountAsync();

            return Ok(new
            {
                totalCustomers,
                totalContacts,
                totalNotes
            });
        }
    }
}