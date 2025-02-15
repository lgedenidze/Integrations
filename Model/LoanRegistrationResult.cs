using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class LoanRegistrationResult
    {
        public int? LoanID { get; set; }

        public int ResultCode { get; set; }

        public string Description { get; set; }    

    }
}
