using Microsoft.AspNetCore.Mvc;
using System;

namespace Integrations.Model
{
    public class TrancheSchedules
    {
        public int TrancheId { get; set; }
        public string? MerchantName { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? TransactionAmount { get; set; }
        public decimal? RemainingDebt { get; set; }
        public InitialSchedule InitialSchedule { get; set; }
        public RemainingSchedule RemainingSchedule { get; set; }

    }
}
