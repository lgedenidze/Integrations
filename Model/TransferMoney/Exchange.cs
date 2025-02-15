using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class Exchange
    {
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string CurrencyFix { get; set; }
    }
}
