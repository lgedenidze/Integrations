using Integrations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface ICardsRepository
    {
        public Task<GeneralResult> ResetPin(ResetCardPinParams resetCardPinParams);

        public Task<GeneralResult> Block(BlockCardParams blockCardParams);

        public Task<GeneralResult> Unblock(UnblockCardParams unblockCardParams);

        public Task<GeneralResult> ChangeCurrencyPriority(ChangeCardCurrencyPriorityParams changeCardCurrencyPriorityParams);

        public Task<GeneralResult> ChangeHighRiskTransactionStatus(ChangeCardHighRiskTransactionStatusParams changeCardHighRiskTransactionStatusParams);

        public Task<GeneralResult> AddCard(AddCardParams addCardParams);
        public Task<GeneralResult> LinkInstantCardToAccount(LinkInstantCardToAccountParams linkInstantCardToAccountParams);


    }
}
