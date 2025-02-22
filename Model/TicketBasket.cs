using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Integrations.Model
{
    public class TicketBasket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Event")]
        public int EventId { get; set; }
        [JsonIgnore]
        public Event Event { get; set; }

        public decimal Price { get; set; }
        public int TotalTickets { get; set; }
        public int SoldTickets { get; set; } = 0;

        [JsonIgnore]
        public bool IsSoldOut => SoldTickets >= TotalTickets;
    }
}
