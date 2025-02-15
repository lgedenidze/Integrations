namespace Integrations.Model
{
    public class GetDepositPreOpeningDetailsResult
    {
        public decimal? EffectiveRate { get; set; }
        public decimal? EffectiveRateInGel { get; set; } 
        public decimal? InterestRate { get; set; } 
        public decimal? EarlyClosureInterestRate { get; set; }
        public decimal? RateOnAmount { get; set; } 
        public decimal? WithdrawalPrincipalCommission { get; set; } 
        public string  InterestType { get; set; } 
        public int? InterestPeriod { get; set; } 
        public string InterestPeriodType { get; set; }
        public decimal? MinFirstAmount { get; set; } 
        public decimal? MinimumMonthlyAdd { get; set; }


    }
}
