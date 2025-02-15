using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class AddCurrencyParams
    {
        public string iban { get; set; }
        public List<Currency> currencies { get; set; }
    }
}
