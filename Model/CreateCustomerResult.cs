using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class CreateCustomerResult
    {
        public Error error { get; set; }
        public int CustomerId { get; set; }
    }
}
