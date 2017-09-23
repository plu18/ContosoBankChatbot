using ContosoBankChatbot.Data;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ContosoBankChatbot.Dialogs
{
    [Serializable]
    [LuisModel("5f824806-329e-4d7e-9e4e-204481e994b5", "3bae4fbe7b7d4f4292f8316ccb95399e")]
    public class LoggedinLuisDialog : LuisDialog<object>
    {

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task HandleNone(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Greetings")]
        public async Task HandleGreetingsIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hello, Ask me anything about Contoso Bank.");

            //var reply = context.MakeMessage();
            //var options = new[]
            //{
            //    "Check my balance", "Make a transfer", "Help"
            //};
            //reply.AddHeroCard(
            //    "Welcome to Contoso Bank! What do you want?",
            //    options);

            //await context.PostAsync(reply);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("CheckBalance")]
        public async Task HandleCheckBalanceIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("PostAsyncCheckBalance");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Transfer")]
        public async Task HandleTransferIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("PostAsyncTransfer");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Help")]
        public async Task HandleHelpIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("PostAsyncHelp");
            context.Wait(this.MessageReceived);
        }

    }
}