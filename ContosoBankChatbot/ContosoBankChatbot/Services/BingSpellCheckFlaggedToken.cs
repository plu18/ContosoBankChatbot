using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoBankChatbot.Services
{
    public class BingSpellCheckFlaggedToken
    {

        public int Offset { get; set; }

        public string Token { get; set; }

        public string Type { get; set; }

        public BingSpellCheckSuggestion[] Suggestions { get; set; }
    }
}