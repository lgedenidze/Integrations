using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class LinkInstantCardToAccountParams
    {
        public string EmbossName { get; set; }
        public string CardLastFourDigits { get; set; }
        public string AccountId { get; set; }
    }
}
