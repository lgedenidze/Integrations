using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class ResetCardPinParams
    {
        public int customerId { get; set; }
        public int cardId { get; set; }
    }
}
