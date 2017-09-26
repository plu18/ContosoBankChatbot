using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoBankChatbot.Data
{
    public class BankAccount
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public double Balance { get; set; }
        public bool isDeleted { get; set; }
        
    }
}
