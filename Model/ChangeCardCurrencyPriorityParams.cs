using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class ChangeCardCurrencyPriorityParams
    {
        public int customerId { get; set; }
        public int cardId { get; set; }
        public List<Currency> currencies { get; set; }
    }
}
