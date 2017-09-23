using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoBankChatbot.Data
{
    public class ConversationDataContext : DbContext
    {
        public ConversationDataContext()
            : base("ConversationDataContextConnectionString")
        {
        }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
    }
}
