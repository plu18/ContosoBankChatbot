using Autofac;
using ContosoBankChatbot.Dialogs;
using ContosoBankChatbot.Helpers;
using ContosoBankChatbot.Loggers;
using ContosoBankChatbot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace ContosoBankChatbot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        
        protected void Application_Start()
        {

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Microsoft.Bot.Connector.IMessageActivity, Data.MessageActivity>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                    .ForMember(dest => dest.FromID, opt => opt.MapFrom(src => src.From.Id))
                    .ForMember(dest => dest.FromName, opt => opt.MapFrom(src => src.From.Name))
                    .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text));

                cfg.CreateMap<Account, Data.BankAccount>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                    .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance));

            });

            this.RegisterBotModules();

            //RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalConfiguration.Configure(WebApiConfig.Register);


        }

        private void RegisterBotModules()
        {
            //builder.RegisterModule(new ReflectionSurrogateModule());

            //builder.RegisterModule<GlobalMessageHandlersBotModule>();
            var builder = new ContainerBuilder();
            

            builder.RegisterType<MessageActivityLogger>().AsImplementedInterfaces().InstancePerDependency();

            builder.Update(Conversation.Container);

        }
    }
}
