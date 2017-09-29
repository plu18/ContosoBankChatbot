using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using AdaptiveCards;
using System.Threading;
using ContosoBankChatbot.Models;
using Newtonsoft.Json;
using ContosoBankChatbot.AdaptiveCards;
using System.Text.RegularExpressions;
using AutoMapper;
using ContosoBankChatbot.Utils;

namespace ContosoBankChatbot.Dialogs
{
    [Serializable]
    public class SignInDialog : IDialog<string>
    {

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Entry what user name, Email and phone number do you want to use?");
            await GetSignInAdaptiveCardAttachment(context);
            context.Wait(this.MessageReceivedAsync);

        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            Activity activity = await result as Activity;

            if (!String.IsNullOrWhiteSpace(activity.Text))
            {
                PromptDialog.Choice(context, this.OnOptionSelected,
                    new List<string>() { Constants.YesOption, Constants.NoOption },
                    $"Do you want to me to do the '{activity.Text}'? " +
                    $"This will let me to quit the sign in dialog?");
            }
            else
            {
                Account account = (Account)JsonConvert.DeserializeObject<Account>(activity.Value.ToString());

                Regex regexEmail = new Regex(BotAssets.RegexConstants.Email);
                Regex regexPhone = new Regex(BotAssets.RegexConstants.Phone);

                if (!String.IsNullOrWhiteSpace(account.UserName)
                    && !String.IsNullOrWhiteSpace(account.Email)
                    && !String.IsNullOrWhiteSpace(account.PhoneNumber))
                {
                    if (!regexEmail.IsMatch(account.Email))
                        await context.PostAsync($" Your input Email {account.Email} is not available! ");

                    if (!regexPhone.IsMatch(account.PhoneNumber))
                        await context.PostAsync($" Your input Phone Number {account.PhoneNumber} is not available! ");

                    if (regexEmail.IsMatch(account.Email) && regexPhone.IsMatch(account.PhoneNumber))
                    {
                        using (Data.ContosoBankDataContext dataContext = new Data.ContosoBankDataContext())
                        {
                            var queryUserName = from accountData in dataContext.BankAccounts
                                                where accountData.UserName == account.UserName
                                                select accountData;

                            var queryEmail = from accountData in dataContext.BankAccounts
                                             where accountData.Email == account.Email
                                             select accountData;

                            var queryPhone = from accountData in dataContext.BankAccounts
                                             where accountData.PhoneNumber == account.PhoneNumber
                                             select accountData;
                            //Data.BankAccount queryAccount = query.SingleOrDefault();

                            if (queryUserName.Any())
                                await context.PostAsync($"User name '{account.UserName}' has been used");

                            if (queryEmail.Any())
                                await context.PostAsync($"Email '{account.Email}' has been used");

                            //phone number has been used
                            if (queryPhone.Any())
                                await context.PostAsync($"Phone Number '{account.PhoneNumber}' has been used");

                            //Create new account
                            if (!queryUserName.Any() && !queryEmail.Any() && !queryPhone.Any())
                            {
                                var newAccount = Mapper.Map<Account, Data.BankAccount>(account);
                                if (string.IsNullOrEmpty(newAccount.Id))
                                    newAccount.Id = Guid.NewGuid().ToString();
                                dataContext.BankAccounts.Add(newAccount);
                                dataContext.SaveChanges();

                                await context.PostAsync($"Dear {account.UserName}, you have been signed in a new bank account. " +
                                                $"Your Email is {account.Email}. " +
                                                $"Your phone is {account.PhoneNumber}");

                                context.ConversationData.SetValue(Constants.isLoginKey, true);
                                context.ConversationData.SetValue(Constants.UserNameKey, account.UserName);
                                context.ConversationData.SetValue(Constants.UserEmailKey, account.Email);
                                context.ConversationData.SetValue(Constants.UserPhoneNumberKey, account.PhoneNumber);

                            }
                        }
                    }
                }

                bool isLogin = false;
                context.ConversationData.TryGetValue(Constants.isLoginKey, out isLogin);
                if (isLogin)
                {
                    string UserName;
                    context.ConversationData.TryGetValue(Constants.UserNameKey, out UserName);
                    await context.PostAsync($"Dear {UserName}, Thanks for login Contoso Bank. ");
                    context.Done<object>(null);
                }
                else
                {
                    await context.PostAsync($"Try again please.");
                    await GetSignInAdaptiveCardAttachment(context);
                }
            } 
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            string optionSelected = await result;

            switch (optionSelected)
            {
                case Constants.YesOption:
                    context.Done<object>(null);
                    break;

                case Constants.NoOption:
                    await GetSignInAdaptiveCardAttachment(context);
                    context.Wait(this.MessageReceivedAsync);
                    break;

            }
        }

        private async Task GetSignInAdaptiveCardAttachment(IDialogContext context)
        {
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = AdaptiveCardFactory.CreateNormalAdaptiveCard(Constants.SignInCard)
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply);
        }

        
    }
}