using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using AdaptiveCards;
using ContosoBankChatbot.Models;
using ContosoBankChatbot.Data;

namespace ContosoBankChatbot.AdaptiveCards
{
    public class AdaptiveCardFactory
    {
        public static AdaptiveCard CreateNormalAdaptiveCard(String type)
        {
            AdaptiveCard adaptiveCard = null;

            if (type.Equals(Constants.SignInCard))
            {
                adaptiveCard = GetSignInCard();
            }
            else if (type.Equals(Constants.LoginCard))
            {
                adaptiveCard = GetLoginCard();
            }

            return adaptiveCard;
        }

        public static AdaptiveCard CreateStockAdaptiveCard(
            StockPrice stockPrice)
        {
            AdaptiveCard adaptiveCard = null;
            adaptiveCard = GetStockAdaptiveCard(stockPrice);
            return adaptiveCard;
        }

        public static AdaptiveCard CreateUserInfoAdaptiveCard(BankAccount bankAccount)
        {
            AdaptiveCard adaptiveCard = null;
            adaptiveCard = GetUserInfoCard(bankAccount);
            return adaptiveCard;
        }

        private static AdaptiveCard GetExchangeAdaptiveCard()
        {

            return new AdaptiveCard()
            {

            };
        }

        private static AdaptiveCard GetUserInfoCard(BankAccount bankAccount)
        {
            return new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                    new ColumnSet()
                    {
                        Columns = new List<Column>()
                        {
                            new Column()
                            {
                                Size = ColumnSize.Stretch,
                                Items = new List<CardElement>()
                                {
                                    new FactSet()
                                    {
                                        Facts = new List<Fact>()
                                        {
                                            new Fact() { Title = "User Name", Value = $"{bankAccount.UserName.ToString()}"},
                                            new Fact() { Title = "Email", Value = $"{bankAccount.Email.ToString()}"},
                                            new Fact() { Title = "Phone Number", Value = $"{bankAccount.PhoneNumber.ToString()}"},
                                            new Fact() { Title = "Balance", Value = $"{bankAccount.Balance.ToString()}"}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            };
        }

        private static AdaptiveCard GetSignInCard()
        {
            return new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                    new TextBlock()
                    {
                        Text = "Sign In",
                        Weight = TextWeight.Bolder,
                        Size = TextSize.Large
                    },
                    new TextInput()
                    {
                        Id = "username",
                        Placeholder = "Input your user name",
                        Style = TextInputStyle.Text
                    },
                    new TextInput()
                    {
                        Id = "email",
                        Placeholder = "Input your Email",
                        Style = TextInputStyle.Email
                    },
                    new TextInput()
                    {
                        Id = "phonenumber",
                        Placeholder = "Input your phone number",
                        Style = TextInputStyle.Tel
                    }
                },
                Actions = new List<ActionBase>()
                {
                    new SubmitAction()
                    {
                        Title = "Sign In",
                        DataJson = "{ \"Type\": \"SignInSubmit\" }"

                    }
                }
            };

        }


        private static AdaptiveCard GetLoginCard()
        {
            return new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                    new TextBlock()
                    {
                        Text = "Login In",
                        Weight = TextWeight.Bolder,
                        Size = TextSize.Large
                    },
                    new TextInput()
                    {
                        Id = "username",
                        Placeholder = "Input your user name",
                        Style = TextInputStyle.Text
                    },
                    new TextInput()
                    {
                        Id = "email",
                        Placeholder = "Input your Email",
                        Style = TextInputStyle.Email
                    },
                    new TextInput()
                    {
                        Id = "phonenumber",
                        Placeholder = "Input your phone number",
                        Style = TextInputStyle.Tel
                    }
                },
                Actions = new List<ActionBase>()
                {
                    new SubmitAction()
                    {
                        Title = "Login",
                        DataJson = "{ \"Type\": \"LoginSubmit\" }"

                    }
                }
            };

        }

        private static AdaptiveCard GetStockAdaptiveCard(StockPrice stockPrice)
        {
            decimal change = stockPrice.Change;
            decimal changePercentage = (stockPrice.Change / stockPrice.LastTradePrice) * 100;
            TextBlock changeTextBlock = new TextBlock();
            if (change < 0)
            {
                changeTextBlock.Text = $"▼ {Math.Abs(change).ToString("#.##")} ({changePercentage.ToString("#.##")}%)";
                changeTextBlock.Color = TextColor.Attention;
            }
            else
            {
                changeTextBlock.Text = $"▲ {change.ToString("#.##")} ({changePercentage.ToString("#.##")}%)";
                changeTextBlock.Color = TextColor.Good;
            }
            
            changeTextBlock.Size = TextSize.Small;
            

            return new AdaptiveCard()
            {
                Body = new List<CardElement>()
                    {
                    new Container()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = $"{stockPrice.Name.Replace("\"", "")} (NASDAQ: {stockPrice.Symbol.ToUpper().Replace("\"", "")})",
                                IsSubtle = true
                            },
                            new TextBlock()
                            {
                                Text = $"{stockPrice.LastTradeDate.Replace("\"", "")}, {stockPrice.LastTradeTime.Replace("\"", "")} EST",
                                IsSubtle = true
                            }
                        }
                    },
                    new Container()
                    {
                        Items = new List<CardElement>()
                        {
                            new ColumnSet()
                            {
                                Columns = new List<Column>()
                                {
                                    new Column()
                                    {
                                        Size = ColumnSize.Stretch,
                                        Items = new List<CardElement>()
                                        {
                                            new TextBlock()
                                            {
                                                Text = $"{stockPrice.LastTradePrice.ToString("#.##")}",
                                                Size = TextSize.ExtraLarge
                                            },
                                            changeTextBlock,
                                        }
                                    },
                                    new Column()
                                    {
                                        Size = ColumnSize.Auto,
                                        Items = new List<CardElement>()
                                        {
                                            new FactSet()
                                            {
                                                Facts = new List<Fact>()
                                                {
                                                    new Fact() { Title = "Open", Value = $"{stockPrice.Open.ToString("#.##")}"},
                                                    new Fact() { Title = "High", Value = $"{stockPrice.DayHigh.ToString("#.##")}"},
                                                    new Fact() { Title = "Low", Value = $"{stockPrice.DayLow.ToString("#.##")}"}
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

    }
}