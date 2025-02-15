using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class RefinancedLoan
    {
        public string BankCode { get; set; }

        public double? RefinancedLoanAmount { get; set; }

        public double? RefinancedLoanRemainder { get; set; }

        public string AgreementNoOfSilkBank { get; set; }

        public int? LoanIDOfSilkBank { get; set; }

        public string Currency { get; set; }

        public string RefinancedIDFromCreditBureau { get; set; }

        public string RefinancedLoanType { get; set; }     

    }
}
