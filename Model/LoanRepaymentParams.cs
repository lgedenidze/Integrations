using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class LoanRepaymentParams
    {
        public int loanID { get; set; }
        public string accountID { get; set; }
        public decimal amount { get; set; }
    }

}
