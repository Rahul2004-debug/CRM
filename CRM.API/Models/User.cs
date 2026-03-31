using System.ComponentModel.DataAnnotations;

namespace CRM.API.Models
{
    /// <summary>
    /// Represents system users (Admin, Sales Rep)
    /// </summary>
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Role { get; set; }
    }
}