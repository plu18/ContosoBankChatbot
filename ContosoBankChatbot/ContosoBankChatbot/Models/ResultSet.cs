using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoBankChatbot.Models
{
    public class ResultSet
    {
        public string Query { get; set; }
        public lResult[] Result { get; set; }
    }
}