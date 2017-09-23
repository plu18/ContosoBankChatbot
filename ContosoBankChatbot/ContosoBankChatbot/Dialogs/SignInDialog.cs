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
                await GetSignInAdaptiveCardAttachment(context);
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

                    using (Data.ConversationDataContext dataContext = new Data.ConversationDataContext())
                    {
                        var newAccount = Mapper.Map<Account, Data.BankAccount>(account);
                        if (string.IsNullOrEmpty(newAccount.Id))
                            newAccount.Id = Guid.NewGuid().ToString();
                        dataContext.BankAccounts.Add(newAccount);
                        dataContext.SaveChanges();
                    }

                    await context.PostAsync($"Dear {userName}, you have been signed in a new bank account. " +
                                            $"Your Email is {email}. " +
                                            $"Your phone is {phoneNumber}");

                    RootDialog._currentAccount.IsLogin = true;
                    RootDialog._currentAccount.UserName = userName;
                    RootDialog._currentAccount.Email = email;
                    RootDialog._currentAccount.PhoneNumber = phoneNumber;

                    context.Done<object>(null);
                }
                else
                {
                    context.Wait(this.MessageReceivedAsync);
                }
            }
            

            
        }
        
        private async Task GetSignInAdaptiveCardAttachment(IDialogContext context)
        {
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = AdaptiveCardFactory.CreateAdaptiveCard(Constants.SignInCard, null)
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply);
        }

        
    }
}