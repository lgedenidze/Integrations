using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class District
    {
        public int Id { get; set; }
        public string DistrictNameGe { get; set; }
        public string DistrictNameEn { get; set; }
        public int? RegionId { get; set; }
    }
}
