using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Web.Configuration;
using ContosoBankChatbot.Services;
using System;
using System.Diagnostics;
using Autofac;
using Microsoft.Bot.Builder.Internals.Fibers;
using ContosoBankChatbot.Models;
using System.Linq;

namespace ContosoBankChatbot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        private static readonly bool IsSpellCorrectionEnabled = bool.Parse(WebConfigurationManager.AppSettings["IsSpellCorrectionEnabled"]);

        private readonly BingSpellCheckService spellService = new BingSpellCheckService();

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            try
            {
                if (activity.Type == ActivityTypes.Message)
                {
                    if (IsSpellCorrectionEnabled)
                    {
                        try
                        {
                            activity.Text = await this.spellService.GetCorrectedTextAsync(activity.Text);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.ToString());
                        }
                    }
                    await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                }
                else
                {
                    HandleSystemMessage(activity);
                }
            }
            catch (Exception exp)
            {
                var reply = activity.CreateReply($"Error:{exp.Message} - {exp.StackTrace}");
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                await connector.Conversations.ReplyToActivityAsync(reply);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
                //return message.CreateReply("ScottDeleteUserData");
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                //return message.CreateReply("ScottConversationUpdate");
                var user = message.MembersAdded.FirstOrDefault();
                if (user != null)
                {
                    ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                    //var reply = message.CreateReply($"HandleSystemMessage ConversationUpdate");
                    //connector.Conversations.ReplyToActivity(reply);
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
                //return message.CreateReply("ScottContactRelationUpdate");
            }
            else if (message.Type == ActivityTypes.Typing)
            {

                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
                //return message.CreateReply("ScottPing");
            }

            return null;
        }
    }
}