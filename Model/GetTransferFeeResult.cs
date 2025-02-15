using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class GetTransferFeeResult
    {
        public double? fee { get; set; }

        public Error error { get; set; }
    }
}
