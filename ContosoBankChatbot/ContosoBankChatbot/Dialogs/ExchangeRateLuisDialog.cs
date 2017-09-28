using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using AdaptiveCards;
using ContosoBankChatbot.AdaptiveCards;
using ContosoBankChatbot.Models;
using ContosoBankChatbot.Utils;
using Newtonsoft.Json;

namespace ContosoBankChatbot.Dialogs
{
    [Serializable]
    public class ExchangeRateLuisDialog : IDialog<string>
    {
        private const string YesOption = "Yes";
        private const string NoOption = "No";
        private string messageTemp = "";

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Exchange Rate Luis Dialog StartAsync.");
            await CreateExchangeRateCardsAttachment(context);
            context.Wait(this.MessageReceived);
        }

        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            Activity activity = await result as Activity;

            if (!String.IsNullOrWhiteSpace(activity.Text))
            {
                messageTemp = activity.Text;
                await ShowOptions(context, messageTemp);
            }
            else
            {
                ExchangeRateInputModel exRateInput = (ExchangeRateInputModel)JsonConvert
                .DeserializeObject<ExchangeRateInputModel>(activity.Value.ToString());

                if (!String.IsNullOrWhiteSpace(exRateInput.ExchangeRateInputValue)
                    && !String.IsNullOrWhiteSpace(exRateInput.ExchangeFromInputId)
                    && !String.IsNullOrWhiteSpace(exRateInput.ExchangeToInputId))
                {
                    ExchangeRate exRate = await YahooExchangeRate.GetExchangeRate(
                        exRateInput.ExchangeRateInputValue,
                        exRateInput.ExchangeFromInputId,
                        exRateInput.ExchangeToInputId);

                    CurrencyModel exFromDetail = AdaptiveCardFactory.currencyDictionary[exRate.FromCurrency];
                    CurrencyModel exToDetail = AdaptiveCardFactory.currencyDictionary[exRate.ToCurrency];

                    await context.PostAsync(
                        $"{exRate.InputValue.ToString()} {exFromDetail.name_plural} = " +
                        $"{exRate.OutputValue.ToString()} {exToDetail.name_plural}");
                }
                context.Wait(this.MessageReceived);
            }
        }

        private async Task CreateExchangeRateCardsAttachment(IDialogContext context)
        {
            //ExchangeRate exchangeRate = await YahooExchangeRate.GetExchangeRate(strExFrom, strExTo);
            
            Attachment attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = AdaptiveCardFactory.CreateExchangeRateAdaptiveCard()
            };
            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply);
        }

        private async Task ShowOptions(IDialogContext context, string title)
        {
            PromptDialog.Choice(context, this.OnOptionSelected,
                new List<string>() { YesOption, NoOption },
                $"Do you want to me to do the '{title}'? " +
                $"This will let me to quit the exchange rates calculator?");
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case YesOption:
                        context.Done<string>(messageTemp);
                        break;

                    case NoOption:
                        await CreateExchangeRateCardsAttachment(context);
                        context.Wait(this.MessageReceived);
                        break;

                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceived);
            }
        }
    }
}