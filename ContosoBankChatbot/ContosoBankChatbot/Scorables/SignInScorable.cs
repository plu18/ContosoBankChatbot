using AdaptiveCards;
using ContosoBankChatbot.AdaptiveCards;
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

            return new List<Attachment>
            {
                new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = AdaptiveCardFactory.CreateNormalAdaptiveCard(Constants.SignInCard)
                }
            };
        }
    }
}


