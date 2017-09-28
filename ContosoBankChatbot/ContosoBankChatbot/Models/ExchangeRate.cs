using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoBankChatbot.Models
{
    public class ExchangeRate
    {
        public decimal InputValue { get; set; }
        public decimal OutputValue { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal LastPrice { get; set; }
        public string LastTradeDate { get; set; }
        public string LastTradeTime { get; set; }
    }
}