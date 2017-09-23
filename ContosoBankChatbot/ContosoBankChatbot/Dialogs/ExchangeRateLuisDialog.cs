using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace ContosoBankChatbot.Dialogs
{
    [Serializable]
    [LuisModel("5f824806-329e-4d7e-9e4e-204481e994b5", "3bae4fbe7b7d4f4292f8316ccb95399e")]
    public class ExchangeRateLuisDialog : LuisDialog<object>
    {
        public override async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("What can I help you for current exchange checking?");
            
            context.Wait(this.MessageReceived);

        }

        [LuisIntent("CheckExchangeRate")]
        public async Task HandleCheckExchangeRateIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("HandleCheckExchangeRateIntent");
            
            context.Wait(this.MessageReceived);
        }
    }
}