﻿using AutoMapper;
using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ContosoBankChatbot.Loggers
{
    public class MessageActivityLogger : IActivityLogger
    {
        Task IActivityLogger.LogAsync(IActivity activity)
        {
            IMessageActivity msg = activity.AsMessageActivity();
            using (Data.ConversationDataContext dataContext = new Data.ConversationDataContext())
            {
                var newActivity = Mapper.Map<IMessageActivity, Data.Activity>(msg);
                if (string.IsNullOrEmpty(newActivity.Id))
                    newActivity.Id = Guid.NewGuid().ToString();
                dataContext.Activities.Add(newActivity);
                dataContext.SaveChanges();
            }
            return Task.CompletedTask;
        }
    }
}