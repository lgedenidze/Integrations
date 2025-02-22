using Integrations.Model.Integrations.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Integrations.Model
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Event")]
        public int EventId { get; set; }
        [JsonIgnore]
        public Event Event { get; set; }

        [ForeignKey("TicketBasket")]
        public int BasketId { get; set; }
        [JsonIgnore]
        public TicketBasket Basket { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        public bool IsPaid { get; set; }
        public string QRCodeUrl { get; set; }
    }
}
