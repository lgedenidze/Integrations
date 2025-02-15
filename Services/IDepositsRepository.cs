using Integrations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface IDepositsRepository
    {
        public Task<GeneralResult> OpenDeposit(OpenDepositParams openDepositParams);
        public Task<GetDepositPreOpeningDetailsResult> GetDepositPreOpeningDetails(int customerId,
                                                                                   string depositType,
                                                                                   string currency,
                                                                                   int periodInMonths,
                                                                                   string depositSubType);
    }
}
