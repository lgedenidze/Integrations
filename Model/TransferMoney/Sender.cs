using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class Sender
    {
        //[Required]
        public int? CustomerId { get; set; }

        //[Required]
        public string AccountId { get; set; }
    }
}
