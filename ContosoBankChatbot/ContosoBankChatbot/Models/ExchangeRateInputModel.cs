using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoBankChatbot.Models
{
    public class ExchangeRateInputModel
    {
        public string Type { get; set; }
        public string ExchangeRateInputValue { get; set; }
        public string ExchangeFromInputId { get; set; }
        public string ExchangeToInputId { get; set; }
    }
}