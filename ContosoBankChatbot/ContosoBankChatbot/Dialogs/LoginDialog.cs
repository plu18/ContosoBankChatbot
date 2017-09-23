using AdaptiveCards;
using AutoMapper;
using ContosoBankChatbot.AdaptiveCards;
using ContosoBankChatbot.Data;
using ContosoBankChatbot.Models;
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
            var message = await result;

            Account account = (Account)JsonConvert.DeserializeObject<Account>(message.Value.ToString());

            String userName = account.UserName;
            String email = account.Email;
            String phoneNumber = account.PhoneNumber;

            Regex regexEmail = new Regex(BotAssets.RegexConstants.Email);
            Regex regexPhone = new Regex(BotAssets.RegexConstants.Phone);

            if (String.IsNullOrWhiteSpace(userName)
                && String.IsNullOrWhiteSpace(email)
                && String.IsNullOrWhiteSpace(phoneNumber))
            {
                await GetLoginAdaptiveCardAttachment(context);
            }
            else
            {
                if (!regexEmail.IsMatch(email))
                {
                    await context.PostAsync($" Your input Email {email} is not available! ");
                }

                if (!regexPhone.IsMatch(phoneNumber))
                {
                    await context.PostAsync($" Your input Phone Number {phoneNumber} is not available! ");

                }

                if (regexEmail.IsMatch(email) && regexPhone.IsMatch(phoneNumber))
                {
                    using (ConversationDataContext dataContext = new ConversationDataContext())
                    {
                        foreach (BankAccount bankAccount in dataContext.BankAccounts)
                        {
                            if (userName == bankAccount.UserName
                                && email == bankAccount.Email
                                && phoneNumber == bankAccount.PhoneNumber)
                            {
                                RootDialog._currentAccount.IsLogin = true;
                                RootDialog._currentAccount.UserName = userName;
                                RootDialog._currentAccount.Email = email;
                                RootDialog._currentAccount.PhoneNumber = phoneNumber;
                                break;
                            }

                        }

                    }

                    if (RootDialog._currentAccount.IsLogin)
                    {
                        await context.PostAsync($"Dear {RootDialog._currentAccount.UserName}, Thanks for login Contoso Bank. ");
                        context.Done<object>(null);
                    }
                    else
                    {
                        await context.PostAsync(" You have not Signned In yet. " +
                                    "Please create an account first. " +
                                    "Or check you login input correct ");
                        
                    }
                }
                else
                {
                    await context.PostAsync("Please try again");
                    await GetLoginAdaptiveCardAttachment(context);
                }
            }
        }


        private async Task GetLoginAdaptiveCardAttachment(IDialogContext context)
        {
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = AdaptiveCardFactory.CreateAdaptiveCard(Constants.LoginCard, null)
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply);
        }

    }
}