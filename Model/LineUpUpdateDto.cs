using Integrations.Model.Enums;
using System;

namespace Integrations.Model
{
    public class LineUpUpdateDto
    {
        public int Id { get; set; } // ✅ Required for updating existing LineUps
        public int EventId { get; set; } // ✅ Required for associating LineUp with Event

        public FloorType Floor { get; set; }
        public string ArtistName { get; set; }
        public DateTime StartTime { get; set; }
        public bool IsHeaderLineUp { get; set; }
    }
}
