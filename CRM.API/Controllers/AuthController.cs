using BCrypt.Net;
using CRM.API.Data;
using CRM.API.DTOs;
using CRM.API.Helpers;
using CRM.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers
{
    /// <summary>
    /// Handles user authentication
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        /// <summary>
        /// Register new user
        /// </summary>
        [HttpPost("register")]
        public IActionResult Register(RegisterDTO dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User registered successfully");

        }
        /// <summary>
        /// User login
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login(LoginDTO dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized("Invalid email");

            bool validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!validPassword)
                return Unauthorized("Invalid password");

            var token = JwtHelper.GenerateToken(user, _config);

            return Ok(new
            {
                token = token,
                role = user.Role,
                name = user.Name
            });
        }
        [Authorize(Roles = "Admin,SalesRep")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customer updatedCustomer)
        {
            var customer = await _context.Customers.FindAsync(id);

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
    }
}