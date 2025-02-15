using System;

namespace Integrations.Model
{
    public class Schedule
    {
        
       public DateTime? CalculationDate { get; set; }
       public DateTime? PaymentDate { get; set; }
       public decimal? InitialAmount { get; set; }
       public decimal? Remainder { get; set; }
       public decimal? BaseAmount { get; set; }
       public decimal? InterestAmount { get; set; }
       public decimal? PaymentAmount { get; set; }
       public decimal? Insurence { get; set; }
       public decimal? Fee { get; set; }

      


    }
}

