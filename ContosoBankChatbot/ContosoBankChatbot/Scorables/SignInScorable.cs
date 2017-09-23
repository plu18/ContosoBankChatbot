using AdaptiveCards;
using ContosoBankChatbot.Scorables;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ContosoBankChatbot.Dialogs
{
    public class SignInScorable : RichCardScorable
    {
        public SignInScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        public override string Trigger
        {
            get
            {
                return "Create a new account";
            }
        }



        protected override IList<Attachment> GetCardAttachments()
        {

            var card = new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                    new TextBlock()
                    {
                        Text = "Entry what user name, Email and phone number do you want to use? ",
                        Weight = TextWeight.Bolder
                    },
                    new TextInput()
                    {
                        Id = "username",
                        Placeholder = "Input your user name",
                        Style = TextInputStyle.Text
                    },
                    new TextInput()
                    {
                        Id = "email",
                        Placeholder = "Input your Email",
                        Style = TextInputStyle.Email
                    },
                    new TextInput()
                    {
                        Id = "phonenumber",
                        Placeholder = "Input your phone number",
                        Style = TextInputStyle.Tel
                    }
                },
                Actions = new List<ActionBase>()
                {
                    new SubmitAction()
                    {
                        Title = "Submit",
                        DataJson = "{ \"Type\": \"SignInSubmit\" }"
                    }
                }
            };

            return new List<Attachment>
            {
                new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card
                }
            };


        }
    }
}


