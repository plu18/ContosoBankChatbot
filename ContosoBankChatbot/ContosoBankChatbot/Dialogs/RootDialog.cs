using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using ContosoBankChatbot.Models;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Resource;
using System.Windows.Input;
using ContosoBankChatbot.Services;
using System.Diagnostics;
using System.Collections.Generic;

namespace ContosoBankChatbot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public static CurrentAccountSingleton _currentAccount = new CurrentAccountSingleton();

        private const string SignInOption = "Create a new account";
        private const string LoginOption = "Already have an account";
        private const string RateCheckOption = "Check current exchange rate";
        private const string StockCheckOption = "Check stock";


        public Task StartAsync(IDialogContext context)
        {
            //await context.PostAsync("Hi, I'm the Contoso Bank bot. Let's get started.");
            context.Wait(this.MessageReceivedAsync);
            return Task.CompletedTask;
        }



        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            
            await this.SendWelcomeMessageAsync(context);

        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            this.ShowOptions(context);
        }
        

        private async Task SignInDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            await context.PostAsync("after sign in");
            await this.SendWelcomeMessageAsync(context);
            //try
            //{
            //    this.SignInName = await result;

            //    context.Call(new LoginDialog(), this.LoginDialogResumeAfter);
            //}
            //catch (TooManyAttemptsException)
            //{
            //    await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");

            //    await this.SendWelcomeMessageAsync(context);
            //}
        }

        private async Task LoginDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            await context.PostAsync("after login");
            await this.SendWelcomeMessageAsync(context);
        }

        private async Task ExchangeRateDialogResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("after exchange rate");
            await this.SendWelcomeMessageAsync(context);
        }

        private async Task StockDialogResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("after stock");
            await this.SendWelcomeMessageAsync(context);
        }
        

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, 
                new List<string>() { SignInOption, LoginOption, RateCheckOption, StockCheckOption }, 
                "What do you want to do?");
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case SignInOption:
                        context.Call(new SignInDialog(), this.SignInDialogResumeAfter);
                        await context.PostAsync("In SignInDialog");
                        break;

                    case LoginOption:
                        context.Call(new LoginDialog(), this.LoginDialogResumeAfter);
                        await context.PostAsync("In LoginDialog");
                        break;

                    case RateCheckOption:
                        context.Call(new ExchangeRateLuisDialog(), this.ExchangeRateDialogResumeAfter);
                        await context.PostAsync("In ExchangeRateLuisDialog");
                        break;

                    case StockCheckOption:
                        context.Call(new StockLuisDialog(), this.StockDialogResumeAfter);
                        await context.PostAsync("In StockLuisDialog");
                        break;
                        
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}