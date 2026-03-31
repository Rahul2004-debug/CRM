namespace CRM.API.DTOs
{
    public class CreateContactDTO
    {
        public int CustomerId { get; set; }

        public string Name { get; set; }

        public string Position { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}