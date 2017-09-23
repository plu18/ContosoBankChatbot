using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoBankChatbot.Data
{
    public class Activity
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string FromID { get; set; }
        public string FromName { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
