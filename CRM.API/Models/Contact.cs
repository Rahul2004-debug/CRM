using System.ComponentModel.DataAnnotations;

namespace CRM.API.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }      // SalesRep
        public int CustomerId { get; set; }  // Manual input

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Position { get; set; }
    }
}