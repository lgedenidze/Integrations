using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class BlockCardParams
    {
        public int customerId { get; set; }
        public int cardId { get; set; }

        public CardBlockType cardBlockType { get; set; }
    }


    public enum CardBlockType
    {
        CANCEL,
        LOST,
        STOLEN
    }
}
