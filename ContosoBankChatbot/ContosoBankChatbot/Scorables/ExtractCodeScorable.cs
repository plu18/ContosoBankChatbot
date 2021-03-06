﻿using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;

namespace ContosoBankChatbot.Scorables
{
    public abstract class ExtractCodeScorable : TriggerScorable
    {
        private const string TypeAttrib = "type";

        public ExtractCodeScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }
        public ExtractCodeScorable(IDialogTask task) : base(task)
        {
        }

        private void SaveCode(string sourceFile, string key)
        {
            var commandsCode = XElement.Load(HostingEnvironment.MapPath(sourceFile));
            var element = (XElement)commandsCode
                .Nodes()
                .FirstOrDefault(n => (n as XElement).Attribute(TypeAttrib).Value.Equals(this.GetType().Name));

            if (element != null)
            {
                this.BotData.PrivateConversationData.SetValue(key, element.Value);
            }
            else
            {
                this.BotData.PrivateConversationData.RemoveValue(key);
            }
        }
    }
}