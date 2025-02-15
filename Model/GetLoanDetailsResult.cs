using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class GetLoanDetailsResult
    {
        public decimal? MonthlyPayment { get; set; }
        public DateTime? LastPayDate { get; set; }
        public DateTime? NextPayDate { get; set; }
        public decimal? NextPayAmount { get; set; }
        public decimal? EffectiveRate { get; set; }
        public decimal? PayAmountEndOfTerm { get; set; }
        public decimal? TotalPayment { get; set; }
        public decimal? ReceiveAmount { get; set; }

        public decimal? CustomerCommission { get; set; }
        public decimal? GiveOutFee { get; set; }

        public decimal? AdvancePayInt { get; set; }

        public decimal? OverduePayInt { get; set; }

        public string AdditionalInfo { get; set; }
        public string FullName { get; set; }
        public string ContactPersonFullName { get; set; }
        public string LegalAddress { get; set; }
        public string ActualAddress { get; set; }
        public string PersonalNum { get; set; }
        public string MobileNumber { get; set; }
        public string AgreementId { get; set; }


        public string LoanPurpose { get; set; }
        public decimal? AnnualInterestRate { get; set; }
        public decimal? LoanAmount { get; set; }
        public DateTime? FirstPaymentDate { get; set; }
        public decimal? FirstPaymentAmount { get; set; }
        public string Currency { get; set; }
        public decimal? LoanRemainMonths { get; set; }

        //მერე
        public decimal? LimitAmount { get; set; }
        public decimal? UsedCredit { get; set; }
        public int? BillingDay { get; set; }
        public int? PaymentDay { get; set; }
        public decimal? RecommendedPaymentAmount { get; set; }
        public decimal? MinimumPaymentAmount { get; set; }
        public DateTime? LimitIssueDate { get; set; }
        public DateTime? LimitExpireDate { get; set; }
        public string IdentDocumentId { get; set; }
        public string ClientEmail { get; set; }
        public decimal? ClientPayIfAtm { get; set; }
        public decimal? ClientPayIfMin { get; set; } 
        public string? ServiceAccount { get; set; }
        public decimal? EffectiveNonCash { get; set; }
        public decimal? EffectiveWithdrawal { get; set; }
        public decimal? EffectiveRecommended { get; set; }
        public decimal? EffectiveProlong { get; set; }
        public decimal? LoanTerm { get; set; }
        public decimal? MinimumPaymentAmountFixed { get; set; }
        





    }
}
