using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ContosoBankChatbot.BotAssets.Dialogs
{
    [Serializable]
    public abstract class PagedCarouselDialog<T> : IDialog<T>
    {
        private int pageNumber = 1;
        private int pageSize = 5;

        public virtual string Prompt { get; }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(this.Prompt ?? "Please select an item");

            await this.ShowProducts(context);

            context.Wait(this.MessageReceivedAsync);
        }

        public abstract PagedCarouselCards GetCarouselCards(int pageNumber, int pageSize);

        public abstract Task ProcessMessageReceived(IDialogContext context, string message);

        protected async Task ShowProducts(IDialogContext context)
        {
            var reply = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = new List<Attachment>();

            var productsResult = this.GetCarouselCards(this.pageNumber, this.pageSize);
            foreach (HeroCard productCard in productsResult.Cards)
            {
                reply.Attachments.Add(productCard.ToAttachment());
            }

            await context.PostAsync(reply);

            if (productsResult.TotalCount > this.pageNumber * this.pageSize)
            {
                await this.ShowMoreOptions(context);
            }
        }

        protected async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            // TODO: validation
            if (message.Text.Equals("Yes, please!", StringComparison.InvariantCultureIgnoreCase))
            {
                this.pageNumber++;
                await this.StartAsync(context);
            }
            else
            {
                await this.ProcessMessageReceived(context, message.Text);
            }
        }

        private async Task ShowMoreOptions(IDialogContext context)
        {
            var moreOptionsReply = context.MakeMessage();
            moreOptionsReply.Attachments = new List<Attachment>
            {
                    new HeroCard()
                {
                    Text = "Want more options?",
                    Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.ImBack, "Yes, please!", value: "Yes, please!")
                    }
                }.ToAttachment()
            };

            await context.PostAsync(moreOptionsReply);
        }

        public class PagedCarouselCards
        {
            public IEnumerable<HeroCard> Cards { get; set; }

            public int TotalCount { get; set; }
        }
    }
}
