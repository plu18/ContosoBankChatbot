using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoBankChatbot.Models
{
    public class CurrentAccountSingleton
    {

        public bool IsLogin { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public double Balance { get; set; }
        
    }
}