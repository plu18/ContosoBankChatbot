using Autofac;
using ContosoBankChatbot.Scorables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoBankChatbot.Helpers
{
    public static class CommandsHelper
    {
        public static IEnumerable<Type> GetRegistrableTypes()
        {
            return typeof(TriggerScorable)
                .Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(TriggerScorable).IsAssignableFrom(t))
                .OrderBy(t => t.Name);
        }

        public static IEnumerable<string> GetValidTriggers(ILifetimeScope scope)
        {
            return CommandsHelper
                .GetRegistrableTypes()
                .Select(t => scope.ResolveKeyed(t.Name, t) as TriggerScorable)
                .Select(c => c.Trigger);
        }
    }
}