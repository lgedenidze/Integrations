using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class TransferMoneyResult
    {
        public bool success { get; set; }
        public Error error { get; set; }
    }
}
