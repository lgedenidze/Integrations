using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class OpenAccountResult
    {
        public Error error { get; set; }
        public string iban { get; set; }
    }
}
