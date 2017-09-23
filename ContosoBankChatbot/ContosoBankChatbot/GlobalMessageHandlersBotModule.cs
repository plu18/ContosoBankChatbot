using Autofac;
using ContosoBankChatbot.Dialogs;
using ContosoBankChatbot.Helpers;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;

namespace ContosoBankChatbot
{
    public class GlobalMessageHandlersBotModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            
            foreach (var commandType in CommandsHelper.GetRegistrableTypes())
            {
                builder
                    .RegisterType(commandType)
                    .Keyed(commandType.Name, commandType)
                    .AsImplementedInterfaces()
                    .InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
            }

        }
    }
}