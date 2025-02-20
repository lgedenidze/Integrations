using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    using Swashbuckle.AspNetCore.Annotations;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

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
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public string Password { get; set; } // ✅ Store hashed password

            public string PhoneNumber { get; set; } // Optional

            [Required]
            public DateTime Birthdate { get; set; }

            public string SocialMediaProfileLink { get; set; }

            [Required]
            public string Country { get; set; }

            [Required, StringLength(20)]
 
            [SwaggerSchema(ReadOnly = true)]
            public bool IsVerified { get; set; } = false; // ✅ Default to false
            [SwaggerSchema(ReadOnly = true)]
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            [SwaggerSchema(ReadOnly = true)]
            public string Role { get; set; } = "User";
        }
    }

}
