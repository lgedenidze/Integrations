using System.Collections.Generic;

namespace Integrations.Model
{
    public class GetLoanScheduleResult
    {
        public int  LoanId { get; set; }
        public List<Schedule> Schedules { get; set; }
        public Error Error { get; set; }

    }
}
