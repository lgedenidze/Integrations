using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class KYCResult
    {
        public Error error { get; set; }
        public bool success { get; set; }
        public int? kycId { get; set; }
    }
}
