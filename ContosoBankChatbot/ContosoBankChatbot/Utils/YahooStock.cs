using ContosoBankChatbot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using CsvHelper;

namespace ContosoBankChatbot.Utils
{
    public class YahooStock
    {

        public static async Task<StockPrice> GetStock(string strStock)
        {
            string strRet = string.Empty;
            StockPrice dblStock = await YahooStock.GetStockPriceAsync(strStock);
            
            if (dblStock.Name == "N/A")   // might be a company name rather than a stock ticker name
            {
                string strTicker = await GetStockTickerName(strStock);
                if (string.Empty != strTicker)
                {
                    dblStock = await YahooStock.GetStockPriceAsync(strTicker);
                    strStock = strTicker;
                }
            }
            

            return dblStock;
        }

        public static async Task<StockPrice> GetStockPriceAsync(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return null;

            string nameUrl = $"http://finance.yahoo.com/d/quotes.csv?s={symbol}&f=n";
            string url = $"http://finance.yahoo.com/d/quotes.csv?s={symbol}&f=sd1t1c1l1ohg";
            string nameCsv;
            string csv;
            using (WebClient client = new WebClient())
            {
                nameCsv = await client.DownloadStringTaskAsync(nameUrl).ConfigureAwait(false);
                csv = await client.DownloadStringTaskAsync(url).ConfigureAwait(false);
            }

            string nameLine = nameCsv.Split('\n')[0];
            string line = csv.Split('\n')[0];
            string[] cols = line.Split(',');
            
            StockPrice price = new StockPrice();

            price.Name = nameLine;
            price.Symbol = cols[0];
            price.LastTradeDate = cols[1];
            price.LastTradeTime = cols[2];
            if (cols[3] == "N/A")
                price.Change = 0;
            else
                price.Change = Convert.ToDecimal(cols[3]);

            if (cols[4] == "N/A")
                price.LastTradePrice = 0;
            else
                price.LastTradePrice = Convert.ToDecimal(cols[4]);

            if (cols[5] == "N/A")
                price.Open = 0;
            else
                price.Open = Convert.ToDecimal(cols[5]);

            if (cols[6] == "N/A")
                price.DayHigh = 0;
            else
                price.DayHigh = Convert.ToDecimal(cols[6]);

            if (cols[7] == "N/A")
                price.DayLow = 0;
            else
                price.DayLow = Convert.ToDecimal(cols[7]);
            
            return price;
        }
        

        public static async Task<string> GetStockTickerName(string strCompanyName)
        {
            string strRet = string.Empty;
            string url = $"http://d.yimg.com/autoc.finance.yahoo.com/autoc?" +
                            $"query={strCompanyName}&" +
                            $"region=1&" +
                            $"lang=en&" +
                            $"callback=YAHOO.Finance.SymbolSuggest.ssCallback";
            string sJson = string.Empty;
            using (WebClient client = new WebClient())
            {
                sJson = await client.DownloadStringTaskAsync(url).ConfigureAwait(false);
            }

            sJson = StripJsonString(sJson);
            YhooCompanyLookup lookup = null;
            try
            {
                lookup = JsonConvert.DeserializeObject<YhooCompanyLookup>(sJson);
            }
            catch (Exception e)
            {

            }

            if (null != lookup)
            {
                foreach (lResult r in lookup.ResultSet.Result)
                {
                    if (r.exchDisp == "NASDAQ")
                    {
                        strRet = r.symbol;
                        break;
                    }
                }
            }

            return strRet;
        }

        // String retrurned from Yahoo Company name lookup contains more than raw JSON
        // strip off the front/back to get to raw JSON
        private static string StripJsonString(string sJson)
        {
            int iPos = sJson.IndexOf('(');
            if (-1 != iPos)
            {
                sJson = sJson.Substring(iPos + 1);
            }

            iPos = sJson.LastIndexOf(')');
            if (-1 != iPos)
            {
                sJson = sJson.Substring(0, iPos);
            }

            return sJson;
        }
    }


}