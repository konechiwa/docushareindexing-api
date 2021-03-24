namespace DocuShareIndexingAPI.DTOs
{
    public class RegisterDto
    {
        /**
        * @notice properties variables.
        */
        public string Username { get; set; }
        public string Password { get; set; }
        public string ProjectName { get; set; }
        public string CompanyName { get; set; }
    }
}