using System;

namespace Integrations.Model
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Role { get; set; }
        public string Country { get; set; }
        public string PersonalNumber { get; set; }
        public DateTime Birthdate { get; set; }
        public string PhoneNumber { get; set; }
        public string SocialMediaProfileLink { get; set; }
    }
}
