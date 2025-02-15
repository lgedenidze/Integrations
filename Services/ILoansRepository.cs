using Integrations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface ILoansRepository
    {
        public Task<LoanRegistrationResult> RegisterLoan(LoanRegistrationData loanRegistrationData);

        public Task<CloseLoanResult> CloseLoan(int loanID);
        public Task<GiveOutLoanResult> GiveOutLoan(GiveOutLoanParameters giveOutLoanParameters);
        public Task<GetLoanDetailsResult> GetLoanDetails(int loanID);
        public Task<GeneralResult> LoanRepayment(LoanRepaymentParams loanRepaymentParams);
        public Task<GetLoanScheduleResult> GetLoanSchedule(int loanId);
        public Task<GetMereTrancheDetailsResult> GetMereTrancheDetails(int loanId);
        public Task<GeneralResult> ProlongMere(ProlongMereParams prolongMereParams);
    }
}
