using Microsoft.Bot.Connector;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ContosoBankChatbot.Utils
{
    public class StateServiceHandler
    {

        public static async Task<T> GetStateProperty<T>(Activity activity, string propertyName)
        {
            StateClient stateClient = activity.GetStateClient();
            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            return userData.GetProperty<T>(propertyName);
        }

        public static async Task SetStateProperty<T>(Activity activity, string propertyName, T propertyValue)
        {
            StateClient stateClient = activity.GetStateClient();
            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

            userData.SetProperty<T>(propertyName, propertyValue);
            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

        }

        public static async Task ClearState(Activity activity)
        {
            StateClient stateClient = activity.GetStateClient();
            await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
        }

        public static class StateProperties
        {
            public static readonly string IN_DIALOG = "InDialog";
            public static readonly string LOGIN_AS = "LoginAs";
        }
        
    }
}