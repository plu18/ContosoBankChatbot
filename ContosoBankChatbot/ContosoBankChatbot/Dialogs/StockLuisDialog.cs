using AdaptiveCards;
using ContosoBankChatbot.AdaptiveCards;
using ContosoBankChatbot.Models;
using ContosoBankChatbot.Utils;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ContosoBankChatbot.Dialogs
{
    [LuisModel("0dc686b7-7377-429f-8896-fa96223876f4", "3bae4fbe7b7d4f4292f8316ccb95399e")]
    [Serializable]
    public class StockLuisDialog : LuisDialog<object>
    {
        

        [LuisIntent("StockPrice")]
        public async Task StockPrice(IDialogContext context, LuisResult result)
        {
            string strRet = result.Entities[0].Entity;
            context.ConversationData.SetValue<string>("LastStock", strRet);
            await CreateStockCards(context, strRet);

            context.Wait(MessageReceived);
        }

        [LuisIntent("RepeatLastStock")]
        public async Task RepeatLastStock(IDialogContext context, LuisResult result)
        {
            string strRet = string.Empty;
            string strStock = string.Empty;
            if (!context.ConversationData.TryGetValue("LastStock", out strStock))
            {
                strRet = "I don't have a previous stock to look up!";
                await context.PostAsync(strRet);
            }
            else
            {
                await CreateStockCards(context, strRet);
            }
            
            context.Wait(MessageReceived);
        }

        [LuisIntent("IsTheMarketUpOrDown")]
        public async Task MarketTrendStock(IDialogContext context, LuisResult result)
        {
            string strRet = string.Empty;
            string strStock = string.Empty;
            if (!context.ConversationData.TryGetValue("LastStock", out strStock))
            {
                strRet = "I don't have a previous stock to look up!";
                await context.PostAsync(strRet);
            }
            else
            {
                await CreateStockCards(context, strRet);
            }
            context.Wait(MessageReceived);
        }
        private async Task CreateStockCards(IDialogContext context, string strRet)
        {
            StockPrice stockPrice = await YahooStock.GetStock(strRet);
            if (stockPrice.Name != "N/A")
            {
                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = AdaptiveCardFactory.CreateStockAdaptiveCard(stockPrice)
                };
                var reply = context.MakeMessage();
                reply.Attachments.Add(attachment);
                await context.PostAsync(reply);
            }
            else
            {
                await context.PostAsync("Sorry, There is no stock information you want to inquire about.");
            }
        }

        [LuisIntent("Quit")]
        public async Task QuitHandler(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Quit stock");
            context.Done<object>(null);
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task NoneHandler(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry, I don't understand");
            context.Wait(MessageReceived);
        }

        
    }
}
