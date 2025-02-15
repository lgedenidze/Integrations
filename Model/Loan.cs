using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class Loan
    {
        public int LoanId { get; set; }

        public string AgreementNo { get; set; }

        public string LoanType { get; set; }

        public string Currency { get; set; }

        public decimal InterestRate { get; set; }

        public decimal MaxPayment { get; set; }

        public bool IsCollateralized { get; set; }

        public int? OutstandingInstallments { get; set; }

        public int CurrentBadDays { get; set; }

        public string ClientRole { get; set; }

        public decimal PrincipalAmount { get; set; }

        public decimal InterestAmount { get; set; }

        public decimal PenaltyAmount { get; set; }

        public decimal? CreditLimit { get; set; }

    }
}
