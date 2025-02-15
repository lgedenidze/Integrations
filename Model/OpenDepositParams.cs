using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class OpenDepositParams
    {
        public int customerId { get; set; }
        public string depositType { get; set; }
        public string depositSubType { get; set; }
        public string currency { get; set; }
        public double amount { get; set; }
        public int? periodInMonths { get; set; }
        public string accountIdToTransferFrom { get; set; }
        public string capitalizationAccountId { get; set; }        
        public string agreementId { get; set; }
    }
}
