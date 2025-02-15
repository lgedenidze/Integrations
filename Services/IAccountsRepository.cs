using Integrations.Model;
 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Integrations.Model.Enums.BalanceType;

namespace Integrations.Services
{
    public interface IAccountsRepository
    {
        public Task<Accounts> GetAccounts(int customerId);

        public Task<GetBalanceResult> GetBalance(string accountId );

        public Task<OpenAccountResult> OpenAccount(OpenAccountParams openAccountParams);

        public Task<AddCurrencyResult> AddCurrency(AddCurrencyParams addCurrencyParams);

        public Task<AccountsForCrediting> GetAccountsForCrediting(string phoneNumber, string iban, string personalNo);
        public   Task<GeneralResult> SendBalanceValidationToRabbitMQ(string accountId);
    }
}
