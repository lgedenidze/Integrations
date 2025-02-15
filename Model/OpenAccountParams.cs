using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class OpenAccountParams
    {
        public int customerId { get; set; }
        public string requestId { get; set; }
        public List<Currency> currencies { get; set; }
    }
}
