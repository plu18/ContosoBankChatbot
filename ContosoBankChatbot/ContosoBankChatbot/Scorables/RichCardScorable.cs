using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace ContosoBankChatbot.Scorables
{
    public abstract class RichCardScorable : ExtractCodeScorable
    {
        private const string CSharpSamplesRoot = "https://github.com/Microsoft/BotBuilder-Samples/tree/master/CSharp/";
        private const string NodeJsSamplesRoot = "https://github.com/Microsoft/BotBuilder-Samples/tree/master/Node/";
        private const string RichCardsSample = "cards-RichCards";
        private const string CarouselSample = "cards-CarouselCards";
        private const string RichCardsText = "Rich Cards";
        private const string CarouselText = "Carousel";

        public RichCardScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        public RichCardScorable(IDialogTask task) : base(task)
        {
        }

        protected abstract IList<Attachment> GetCardAttachments();

    }
}