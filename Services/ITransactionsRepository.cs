using Integrations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface ITransactionsRepository
    {
        public Task<TransferMoneyResult> TransferMoney(TransferMoneyData transferMoneyData);

        public Task<GetTransferFeeResult> GetTransferFee(string transferType, string debitAccountId, string creditAccountId, bool isStp, decimal amount);

        public Task<GetTreasuryCodeDescrResult> GetTreasuryCodeDescr(string treasuryCode);

        public Task<GeneralResult> Authorize(AuthorizeParams authorizeParams);

    } 
}
