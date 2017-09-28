using ContosoBankChatbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace ContosoBankChatbot.Utils
{
    public class YahooExchangeRate
    {
        public static async Task<ExchangeRate> GetExchangeRate(string inputString, string fromRate, string toRate)
        {
            string strRet = string.Empty;
            ExchangeRate dblStock = await YahooExchangeRate.GetExchangeRateAsync(inputString, fromRate, toRate);
            
            return dblStock;
        }

        public static async Task<ExchangeRate> GetExchangeRateAsync(string inputString, string fromRateSymbol, string toRateSymbol)
        {
            if (string.IsNullOrWhiteSpace(fromRateSymbol) || string.IsNullOrWhiteSpace(toRateSymbol))
                return null;

            string url = $"http://finance.yahoo.com/d/quotes.csv?e=.csv&f=snl1d1t1&s={fromRateSymbol}{toRateSymbol}=x";

            string csv;
            using (WebClient client = new WebClient())
            {
                csv = await client.DownloadStringTaskAsync(url).ConfigureAwait(false);
            }
            
            string line = csv.Split('\n')[0];
            string[] cols = line.Split(',');

            ExchangeRate exRate = new ExchangeRate();

            exRate.InputValue = Convert.ToDecimal(inputString);
            exRate.FromCurrency = cols[1].Replace("\"", "").Split('/')[0];
            exRate.ToCurrency = cols[1].Replace("\"", "").Split('/')[1];
            exRate.LastPrice = Convert.ToDecimal(cols[2]);
            exRate.LastTradeDate = cols[3];
            exRate.LastTradeTime = cols[4];
            exRate.OutputValue = exRate.InputValue * exRate.LastPrice;

            return exRate;
        }

    }
}