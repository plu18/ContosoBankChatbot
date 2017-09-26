using AdaptiveCards;
using AutoMapper;
using ContosoBankChatbot.AdaptiveCards;
using ContosoBankChatbot.Data;
using ContosoBankChatbot.Models;
using ContosoBankChatbot.Utils;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;


namespace ContosoBankChatbot.Dialogs
{
    [Serializable]
    public class LoginDialog : IDialog<string>
    {

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Login Start");
            await GetLoginAdaptiveCardAttachment(context);
            context.Wait(this.MessageReceivedAsync);

        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            Activity activity = await result as Activity;

            Account account = (Account)JsonConvert.DeserializeObject<Account>(activity.Value.ToString());
            

            Regex regexEmail = new Regex(BotAssets.RegexConstants.Email);
            Regex regexPhone = new Regex(BotAssets.RegexConstants.Phone);

            if (!String.IsNullOrWhiteSpace(account.UserName)
                && !String.IsNullOrWhiteSpace(account.Email)
                && !String.IsNullOrWhiteSpace(account.PhoneNumber))
            {
                if (!regexEmail.IsMatch(account.Email))
                    await context.PostAsync($" Your input Email '{account.Email}' is not available! ");

                if (!regexPhone.IsMatch(account.PhoneNumber))
                    await context.PostAsync($" Your input Phone Number '{account.PhoneNumber}' is not available! ");

                if (regexEmail.IsMatch(account.Email) && regexPhone.IsMatch(account.PhoneNumber))
                {
                    using (ContosoBankDataContext dataContext = new ContosoBankDataContext())
                    {
                        var query = from accountData in dataContext.BankAccounts
                                    where accountData.UserName == account.UserName 
                                    && accountData.Email == account.Email
                                    && accountData.PhoneNumber == account.PhoneNumber
                                    && accountData.isDeleted == false
                                    select accountData;
                        
                        if (query.Any())
                        {
                            context.ConversationData.SetValue(Constants.isLoginKey, true);
                            context.ConversationData.SetValue(Constants.UserNameKey, account.UserName);
                            context.ConversationData.SetValue(Constants.UserEmailKey, account.Email);
                            context.ConversationData.SetValue(Constants.UserPhoneNumberKey, account.PhoneNumber);
                            
                        }
                    }
                }
            }
            
            if (context.ConversationData.GetValue<bool>(Constants.isLoginKey))
            {
                string UserName;
                context.ConversationData.TryGetValue(Constants.UserNameKey, out UserName);
                await context.PostAsync($"Dear {UserName}, Thanks for login Contoso Bank. ");
                context.Done<object>(null);
            }
            else
            {
                await context.PostAsync(" You have not Signned In yet. " +
                            "Please create an account first. " +
                            "Or check you login input correct ");
                await context.PostAsync("Please try again");
                await GetLoginAdaptiveCardAttachment(context);
            }
        }


        private async Task GetLoginAdaptiveCardAttachment(IDialogContext context)
        {
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = AdaptiveCardFactory.CreateNormalAdaptiveCard(Constants.LoginCard)
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply);
        }

    }
}