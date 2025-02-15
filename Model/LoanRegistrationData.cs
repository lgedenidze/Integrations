using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class LoanRegistrationData
    {
        public int CustomerNo { get; set; }

        public string AgreementId { get; set; }

        public string LoanType { get; set; }

        public double Amount { get; set; }

        public string Currency { get; set; }

        public int Term { get; set; }

        public DateTime StartDate { get; set; }

        public int PayDay { get; set; }

        public PayPeriodicity PayPeriodicity { get; set; }

        public Kind Kind { get; set; }

        public int? GracePeriodOnPrincipal { get; set; }

        public int? GracePeriodOnInterest { get; set; }

        public GracePeriodType? GracePeriodOnPrincipalType { get; set; }

        public GracePeriodType? GracePeriodOnInterestType { get; set; }

        public double? InterestRate { get; set; }

        public double? GiveOutFee { get; set; }

        public GiveOutFeeType? GiveOutFeeType { get; set; }

        public double? EarlyRepaymentFee { get; set; }

        public double? EffectiveRate { get; set; }

        public double? EffectiveRateFC { get; set; }

        public string FieldOfActivity { get; set; }

        public bool? HasActiveLoansInOtherBanks { get; set; }

        public double? PTI { get; set; }

        public double? PTIExtended { get; set; }

        public bool? IsHedged { get; set; }

        public bool? IsRefinancingNoCash { get; set; }

        public double? RefinancingInterestAndOtherAmount { get; set; }

        public double? RefinancingPrincipalAmount { get; set; }

        public DateTime LoanAproveDate { get; set; }

        public string OfficerID { get; set; }

        public double? InsuranceAmount { get; set; }

        public double? MonthlyFeeAmount { get; set; }

        public bool? IsRefinancing { get; set; }

        public string Channel { get; set; }

        public string SubscriberNo { get; set; }

        public double? MinimalPayAmount { get; set; }

        public string IncomeCalculationSource { get; set; }
        public List<RefinancedLoan> RefinancedLoans { get; set; }

        public decimal? TotalLiabGel { get; set; } // ჯამური ვალდებულება ლარში
        public decimal? TotalLiabUsd { get; set; } // ჯამური ვალდებულება აშშ დოლარში
        public decimal? TotalLiabEur { get; set; } // ჯამური ვალდებულება ევროში

        // External Balance Liabilities
        public decimal? ExtBalLiabGel { get; set; } // ჯამური გარესაბალანსო ვალდებულება ლარში
        public decimal? ExtBalLiabUsd { get; set; } // ჯამური გარესაბალანსო ვალდებულება აშშ დოლარში
        public decimal? ExtBalLiabEur { get; set; } // ჯამური გარესაბალანსო ვალდებულება ევროში

        // Risk Grade
        public string RiskGrade { get; set; } // კრედიტინფოს საკრედიტო რეიტინგი


    }


    public enum PayPeriodicity
    {
        Weekly,
        Monthly,
        Quarterly,
        Yearly,
        EndOfPeriod
    }


    public enum Kind
    {
        Warranty,
        CreditLine,
        Overdraft,
        SimpleLoan
    }

    public enum GracePeriodType
    {
        Month,
        Day
    }

    public enum GiveOutFeeType
    {
        Percent,
        FixedAmount
    }

    


}
