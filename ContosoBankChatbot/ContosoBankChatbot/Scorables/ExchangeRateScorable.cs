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
    public class ExchangeRateScorable : RichCardScorable
    {
        private readonly IDialogTask task;

        public ExchangeRateScorable(IDialogTask task) : base(task)
        {
        }

        public ExchangeRateScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        public override string Trigger
        {
            get
            {
                return "Check current exchange rate";
            }
        }



        protected override IList<Attachment> GetCardAttachments()
        {

            return new List<Attachment>
            {
                new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = AdaptiveCardFactory.CreateExchangeRateAdaptiveCard()
                }
            };
        }

        protected override async Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            var message = item as IMessageActivity;
            if (message != null)
            {
                var exchangeRateDialog = new ExchangeRateDialog();

                var interruption = exchangeRateDialog.Void<object, IMessageActivity>();

                this.task.Call(interruption, null);

                await this.task.PollAsync(token);
            }
        }

    }
}


