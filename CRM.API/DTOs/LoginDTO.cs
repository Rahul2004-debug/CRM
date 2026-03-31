namespace CRM.API.DTOs
{
    /// <summary>
    /// Data used for user login
    /// </summary>
    public class LoginDTO
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}