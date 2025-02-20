using System.Collections.Generic;
using System;

namespace Integrations.Model
{
    public class EventUpdateDto
    {
        public int Id { get; set; } // ✅ Required for updating

        public string Name { get; set; }
        public string Description { get; set; }
        public string EventPhotoUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<LineUpUpdateDto> LineUps { get; set; }
    }

}
