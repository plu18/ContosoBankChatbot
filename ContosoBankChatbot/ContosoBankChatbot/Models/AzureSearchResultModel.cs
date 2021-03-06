﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoBankChatbot.Models
{
    public class AzureSearchResultModel
    {
        //Data model for search
        public class SearchResult
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public Value[] value { get; set; }
        }

        public class Value
        {
            [JsonProperty("@search.score")]
            public float searchscore { get; set; }
            public string imageURL { get; set; }
            public string Name { get; set; }
            public string Era { get; set; }
            public string Description { get; set; }
            public string id { get; set; }
            public string rid { get; set; }
        }
    }
}