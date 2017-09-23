using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;


namespace ContosoBankChatbot.Services
{

    public class BingSpellCheckResponse
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        public BingSpellCheckFlaggedToken[] FlaggedTokens { get; set; }

        public BingSpellCheckError Error { get; set; }
    }
}