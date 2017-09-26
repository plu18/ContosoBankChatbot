using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoBankChatbot.Data
{
    public class ContosoBankDataContext : DbContext
    {
        public ContosoBankDataContext()
            : base("ContosoBankDataContextConnectionString")
        {
        }
        public DbSet<MessageActivity> MessageActivities { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
    }
}
