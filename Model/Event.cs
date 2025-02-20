using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace Integrations.Model
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
 
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string EventPhotoUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<LineUp> LineUps { get; set; }
    }
}
