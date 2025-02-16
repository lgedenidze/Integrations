using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace Integrations.Models
    {
        public class User
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; }

            [Required]
            public string PasswordHash { get; set; } // ✅ Store hashed password

            public string PhoneNumber { get; set; } // Optional

            [Required]
            public DateTime Birthdate { get; set; }

            public string SocialMediaProfileLink { get; set; }

            [Required]
            public string Country { get; set; }

            [Required, StringLength(20)]
            public string PersonalIdNumber { get; set; }

            public bool IsVerified { get; set; } = false; // ✅ Default to false

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
    }

}
