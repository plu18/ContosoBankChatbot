using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoBankChatbot.Models
{
    public class StockPrice
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string LastTradeDate { get; set; }
        public string LastTradeTime { get; set; }
        
        public decimal Change { get; set; }
        public decimal LastTradePrice { get; set; }
        public decimal Open { get; set; }
        public decimal DayHigh { get; set; }
        public decimal DayLow { get; set; }



    }
}