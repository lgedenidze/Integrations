using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class CheckCustomerResult
    {
        public int customerId { get; set; }

        public bool financialNumberMatched { get; set; }

        public bool hasValidIdDocument { get; set; }

        public bool HasActiveCurrenctAccount { get; set; }

        public bool IsKYCValid { get; set; }

    }
}
