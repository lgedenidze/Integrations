using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class Remittance
    {
        public string RemittanceName { get; set; }
        public string RemittanceTransferCode { get; set; }
        public string RemittanceCounterparty { get; set; }
        public string RemittanceCounterpartyCountryCode { get; set; }        
        public bool? SendToThirdParty { get; set;
        }

    }
}
