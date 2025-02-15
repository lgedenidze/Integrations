using System;
using System.Collections.Generic;

namespace Integrations.Model
{
    public class GetMereTrancheDetailsResult
    {
        public int LoanId { get; set; }
        public decimal ProlongCommission { get; set; } 
        public decimal TotalRecommendedPayment { get; set; }
        public DateTime? PayDate { get; set; }
        public List<TrancheSchedules> TrancheSchedules { get; set; }
        public Error Error { get; set; }
    }
}
