using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class GetBalanceResult
    {
        public Error error { get; set; }
        public double? balance { get; set; }
    }
}
