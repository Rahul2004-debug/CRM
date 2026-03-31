namespace CRM.API.DTOs
{
    /// <summary>
    /// Data used to register a new user
    /// </summary>
    public class RegisterDTO
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }
    }
}