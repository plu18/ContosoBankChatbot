using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using AdaptiveCards;
using ContosoBankChatbot.Models;
using ContosoBankChatbot.Data;
using ContosoBankChatbot.Utils;

namespace ContosoBankChatbot.AdaptiveCards
{
    public class AdaptiveCardFactory
    {
        public static Dictionary<string, CurrencyModel> currencyDictionary;

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

        public static AdaptiveCard CreateExchangeRateAdaptiveCard()
        {
            AdaptiveCard adaptiveCard = null;
            adaptiveCard = GetExchangeRateCard();
            return adaptiveCard;
        }

        private static ChoiceSet GetCurrencyChoiceSet(string id, string selectedCode)
        {
            List<Choice> currencyChoiceList = new List<Choice>();
            foreach (KeyValuePair<string, CurrencyModel> currency in currencyDictionary)
            {
                if (selectedCode == currency.Value.code)
                    currencyChoiceList.Add(new Choice { Title = currency.Value.name, Value = currency.Value.code, IsSelected = true});
                else
                    currencyChoiceList.Add(new Choice { Title = currency.Value.name, Value = currency.Value.code });
            }

            ChoiceSet currencyChoiceSet = new ChoiceSet()
            {
                Id = id,
                Style = ChoiceInputStyle.Compact,
                Value = selectedCode,
                Choices = currencyChoiceList
            };

            return currencyChoiceSet;
        }

        private static AdaptiveCard GetExchangeRateCard()
        {
            currencyDictionary = JsonLoader.LoadJsonToCurrency("ContosoBankChatbot.common_currency.json");
            return new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                    new TextBlock()
                    {
                        Text = "Live Exchange Rates",
                        Weight = TextWeight.Bolder,
                        Size = TextSize.Large
                    },
                    new TextBlock()
                    {
                        Text = "From"
                    },
                    new NumberInput()
                    {
                        Id = Constants.ExchangeRateInputValue,
                        Placeholder = "Input",
                        Value = 1
                    },
                    GetCurrencyChoiceSet(Constants.ExchangeFromInputId, "NZD"),
                    new TextBlock()
                    {
                        Text = "To"
                    },
                    GetCurrencyChoiceSet(Constants.ExchangeToInputId, "CNY")
                },
                Actions = new List<ActionBase>()
                {
                    new SubmitAction()
                    {
                        Title = "Convert",
                        DataJson = "{ \"Type\": \"ExchangeRateSubmit\" }"

                    }
                }
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
                                            new Fact() { Title = "Balance", Value = $"$ {bankAccount.Balance.ToString()}"}
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
                changeTextBlock.Text = $"▼ {Math.Abs(Math.Round(change, 2)).ToString()} ({Math.Round(changePercentage, 2).ToString()}%)";
                changeTextBlock.Color = TextColor.Attention;
            }
            else
            {
                changeTextBlock.Text = $"▲ {Math.Round(change, 2).ToString()} ({Math.Round(changePercentage, 2).ToString()}%)";
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
                                                Text = $"{Math.Round(stockPrice.LastTradePrice, 2).ToString()}",
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
                                                    new Fact() { Title = "Open", Value = $"{Math.Round(stockPrice.Open, 2).ToString()}"},
                                                    new Fact() { Title = "High", Value = $"{Math.Round(stockPrice.DayHigh, 2).ToString()}"},
                                                    new Fact() { Title = "Low", Value = $"{Math.Round(stockPrice.DayLow, 2).ToString()}"}
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