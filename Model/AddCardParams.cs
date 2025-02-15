using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class AddCardParams
    {
        public int customerId { get; set; }
        public string cardProductType { get; set; }
        public string iban { get; set; }
        public List<Currency> currencies { get; set; }
        public string cardHolder { get; set; }
        public string giveOutBranchCode { get; set; }
    }
}
