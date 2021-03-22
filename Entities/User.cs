using System;

namespace DocuShareIndexingAPI.Entities
{
    public class User
    {
        /**
        * @notice properties variables
        */
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
    }
}