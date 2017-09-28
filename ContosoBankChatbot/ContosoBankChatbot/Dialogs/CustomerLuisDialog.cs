using AdaptiveCards;
using ContosoBankChatbot.AdaptiveCards;
using ContosoBankChatbot.Data;
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
    [Serializable]
    [LuisModel("5f824806-329e-4d7e-9e4e-204481e994b5", "3bae4fbe7b7d4f4292f8316ccb95399e")]
    public class CustomerLuisDialog : LuisDialog<object>
    {
        private const string YesOption = "Yes";
        private const string NoOption = "No";

        private const string EntityMoney = "builtin.money";
        private const string EntityCurrency = "builtin.currency";


        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task HandleNone(IDialogContext context, IAwaitable<IMessageActivity> activityResult, LuisResult result)
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

        [LuisIntent("Help")]
        public async Task HandleHelpIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("PostAsyncHelp");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("CheckExchangeRate")]
        public async Task HandleCheckExchangeRateIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("PostAsyncCheckExchangeRate");
            context.Call(new ExchangeRateLuisDialog(), this.ExchangeRateDialogResumeAfter);
        }

        private async Task ExchangeRateDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            var message = await result;
            await context.PostAsync($"after exchange rate: {message}");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("CheckStocks")]
        public async Task HandleCheckStocksIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("PostAsyncCheckStocks");
            context.Call(new StockLuisDialog(), this.StockDialogResumeAfter);
        }

        private async Task StockDialogResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("after stock");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("MyInfo")]
        public async Task HandleMyInfoIntent(IDialogContext context, LuisResult result)
        {

            string UserName = context.ConversationData.GetValue<string>(Constants.UserNameKey);
            string UserEmail = context.ConversationData.GetValue<string>(Constants.UserEmailKey);
            string UserPhoneNumber = context.ConversationData.GetValue<string>(Constants.UserPhoneNumberKey);

            using (ContosoBankDataContext dataContext = new ContosoBankDataContext())
            {
                var query = from accountData in dataContext.BankAccounts
                            where accountData.UserName == UserName
                            && accountData.Email == UserEmail
                            && accountData.PhoneNumber == UserPhoneNumber
                            && accountData.isDeleted == false
                            select accountData;
                

                if (query.Any())
                {
                    Attachment attachment = new Attachment()
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = AdaptiveCardFactory.CreateUserInfoAdaptiveCard(query.SingleOrDefault())
                    };
                    var reply = context.MakeMessage();
                    reply.Attachments.Add(attachment);
                    await context.PostAsync(reply);
                }
            }

            

            await context.PostAsync($"PostAsyncMyInfo: {UserName}, {UserEmail}, {UserPhoneNumber}");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Deposit")]
        public async Task HandleDepositIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("PostAsync: Deposit");

            BankAccount accountToUpdate = await accountUpdate(context, result, Constants.Deposit);

            await context.PostAsync($"PostAsync: {accountToUpdate.Balance}");
            context.Wait(this.MessageReceived);
        }

        


        [LuisIntent("Withdraw")]
        public async Task HandleWithdrawIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("PostAsync: Withdraw");

            BankAccount accountToUpdate = await accountUpdate(context, result, Constants.Withdraw);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Transfer")]
        public async Task HandleTransferIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("PostAsync: Transfer");
            context.Wait(this.MessageReceived);
        }

        private async Task ShowOptions(IDialogContext context, string title)
        {
            PromptDialog.Choice(context, this.OnOptionSelected,
                new List<string>() { YesOption, NoOption },
                $"{title}");
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case YesOption:
                        context.Done<object>(null);
                        break;

                    case NoOption:
                        context.Wait(this.MessageReceived);
                        break;

                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceived);
            }
        }

        [LuisIntent("Logout")]
        public async Task HandleLogoutIntent(IDialogContext context, LuisResult result)
        {
            if (context.ConversationData.GetValue<bool>(Constants.isLoginKey))
                context.ConversationData.SetValue(Constants.isLoginKey, false);

            string UserName = context.ConversationData.GetValue<string>(Constants.UserNameKey);
            await ShowOptions(context, $"Dear customer {UserName}, are you sure for Logout");
        }



        [LuisIntent("DeleteAccount")]
        public async Task HandleDeleteAccountIntent(IDialogContext context, LuisResult result)
        {
            BankAccount accountToDelete;
            string UserName = context.ConversationData.GetValue<string>(Constants.UserNameKey);
            string UserEmail = context.ConversationData.GetValue<string>(Constants.UserEmailKey);
            string UserPhoneNumber = context.ConversationData.GetValue<string>(Constants.UserPhoneNumberKey);

            using (ContosoBankDataContext dataContext = new ContosoBankDataContext())
            {
                var accountQuery = from accountData in dataContext.BankAccounts
                    where accountData.UserName == UserName
                    && accountData.Email == UserEmail
                    && accountData.PhoneNumber == UserPhoneNumber
                    && accountData.isDeleted == false
                    select accountData;
                accountToDelete = accountQuery.FirstOrDefault<BankAccount>();
            }

            using (ContosoBankDataContext newDataContext = new ContosoBankDataContext())
            {
                newDataContext.Entry(accountToDelete).State = System.Data.Entity.EntityState.Deleted;
                newDataContext.SaveChanges();
            }
            await context.PostAsync("PostAsyncDeleteAccount");
            await ShowOptions(context, $"Dear customer {UserName}, are you sure to delete is account?");
        }

        private async Task<double> balanceUpdate(LuisResult result)
        {
            EntityRecommendation currencyEntityRecommendation;
            double moneyValue = 0.0;

            if (result.TryFindEntity(EntityMoney, out currencyEntityRecommendation))
                currencyEntityRecommendation.Type = EntityCurrency;

            if (result.Entities.Any())
            {
                object moneyValueObject;

                result.Entities[0].Resolution.TryGetValue("value", out moneyValueObject);
                IConvertible convert = moneyValueObject as IConvertible;

                if (convert != null)
                {
                    moneyValue = convert.ToDouble(null);
                }
                else
                {
                    moneyValue = 0d;
                }
            }

            return moneyValue;
        }


        private async Task<BankAccount> accountUpdate(IDialogContext context, LuisResult result, string type)
        {
            BankAccount accountToUpdate;

            string UserName = context.ConversationData.GetValue<string>(Constants.UserNameKey);
            string UserEmail = context.ConversationData.GetValue<string>(Constants.UserEmailKey);
            string UserPhoneNumber = context.ConversationData.GetValue<string>(Constants.UserPhoneNumberKey);

            using (ContosoBankDataContext dataContext = new ContosoBankDataContext())
            {
                var accountQuery = from accountData in dataContext.BankAccounts
                                   where accountData.UserName == UserName
                                   && accountData.Email == UserEmail
                                   && accountData.PhoneNumber == UserPhoneNumber
                                   && accountData.isDeleted == false
                                   select accountData;
                accountToUpdate = accountQuery.FirstOrDefault<BankAccount>();

            }

            if (accountToUpdate != null)
            {
                if (type == Constants.Deposit)
                    accountToUpdate.Balance += await balanceUpdate(result);
                else if (type == Constants.Withdraw)
                    accountToUpdate.Balance -= await balanceUpdate(result);
            }

            using (ContosoBankDataContext newDataContext = new ContosoBankDataContext())
            {
                newDataContext.Entry(accountToUpdate).State = System.Data.Entity.EntityState.Modified;
                newDataContext.SaveChanges();
            }

            return accountToUpdate;
        }

    }
}