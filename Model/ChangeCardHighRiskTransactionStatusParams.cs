using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class ChangeCardHighRiskTransactionStatusParams
    {
        public int customerId { get; set; }
        public int cardId { get; set; }

        public CardHighRiskTransactionStatus cardHighRiskTransactionStatus { get; set; }
    }


    public enum CardHighRiskTransactionStatus
    {
        FULL,
        RESTRICT_GAMBLING
    }
}
