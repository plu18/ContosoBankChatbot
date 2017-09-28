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

namespace ContosoBankChatbot.Scorables
{
    public abstract class TriggerScorable : ScorableBase<IActivity, string, double>
    {
        protected readonly IBotToUser BotToUser;
        protected readonly IBotData BotData;
        protected readonly IDialogTask BotTask;

        public TriggerScorable(IBotToUser botToUser, IBotData botData)
        {
            SetField.NotNull(out this.BotToUser, nameof(botToUser), botToUser);
            SetField.NotNull(out this.BotData, nameof(botData), botData);
        }
        public TriggerScorable(IDialogTask botTask)
        {
            SetField.NotNull(out this.BotTask, nameof(botTask), botTask);
        }

        

        public abstract string Trigger { get; }

        protected override async Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            await Task.CompletedTask;
        }

        protected override double GetScore(IActivity item, string state)
        {
            return 1.0;
        }

        protected override bool HasScore(IActivity item, string state)
        {
            return state != null;
        }

        protected override async Task<string> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item.AsMessageActivity();

            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                return message.Text;

            }

            return null;
        }

        
    }
}