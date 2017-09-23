using System;
using Microsoft.Bot.Builder.Location;

namespace ContosoBankChatbot.BotAssets
{
    [Serializable]
    class ContosoLocationResourceManager : LocationResourceManager
    {
        public override string ConfirmationAsk => "OK, you want to use {0}. Is that correct? Enter 'yes' or 'no'.";
    }
}
