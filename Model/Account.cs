using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class Account : AccountsBase
    {
        public bool isCard { get; set; }
        public string accountType { get; set; }
        public bool debitEnabled { get; set; }
        public bool creditEnabled { get; set; }

    }
}
