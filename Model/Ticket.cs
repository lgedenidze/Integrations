using Integrations.Model.Integrations.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection;

namespace Integrations.Model
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        [ForeignKey("TicketBasket")]
        public int BasketId { get; set; }
        [JsonIgnore]
        public TicketBasket Basket { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        [JsonIgnore]
        public string Secret { get; set; }
        public bool IsUsedTicket { get; set; }
        public bool IsPaid { get; set; } = false;
        public string QRCodeUrl { get; set; }
    }
}
