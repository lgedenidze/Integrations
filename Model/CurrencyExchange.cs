using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class CurrencyExchange
    {
        public string SourceCurrency { get; set; } // 'VAL' represents the source currency, e.g., EUR

        public string TargetCurrency { get; set; } // 'BAZ' represents the target currency, e.g., GEL

        public decimal Scale { get; set; } // 'SCALE' likely represents the unit amount of the source currency

        public decimal BuyRate { get; set; } // 'RATE_BUY' is the rate at which the source currency is bought

        public decimal SellRate { get; set; } // 'RATE_SELL' is the rate at which the source currency is sold

        public DateTime LastUpdated { get; set; } // 'UPDATED' is the timestamp of the last update to the rates

        public string ExchangeType { get; set; } // 'COURSE_TYPE' indicates the context of the exchange, e.g., CASH


    }
}
