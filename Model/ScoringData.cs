using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class ScoringData
    {
        public bool IsInsider { get; set; }

        public bool HasRestrictions { get; set; }

        public bool IsBankEmployee { get; set; }

        public bool IsPayroll { get; set; }

        public bool IsBankProblemClient { get; set; }

        public bool IsInCort { get; set; }

        public bool IsInRejectedList { get; set; }

        public bool HasRestructuringLoan { get; set; }

        public bool HadRestructuringLoan { get; set; }

        public decimal SumUnauthorizedAmount { get; set; }

        public decimal SumLiabilitiesAmount { get; set; }

        public int MaxBadDays { get; set; }

        public string FirstLoanDate { get; set; }

        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string LastRestructuringDate { get; set; }

        public int ActiveLoanDays { get; set; }

        public int? CurrentRestructuringLoanDays { get; set; }

        public int? PastRestructuringLoanDays { get; set; }

        public List<Loan> Loans { get; set; }

    }


}
