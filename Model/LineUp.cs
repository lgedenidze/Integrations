using Integrations.Model.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace Integrations.Model
{
    public class LineUp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
 
        public int Id { get; set; }

        [ForeignKey("Event")]
       
        public int EventId { get; set; }
        [JsonIgnore]
        public Event Event { get; set; }
        public FloorType Floor { get; set; }
        public string ArtistName { get; set; }
        public DateTime StartTime { get; set; }

        public bool IsHeaderLineUp { get; set; }

    }
}
