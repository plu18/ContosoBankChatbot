using ContosoBankChatbot.Data;
using ContosoBankChatbot.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using static ContosoBankChatbot.Models.AzureSearchResultModel;

namespace ContosoBankChatbot.Services
{
    [Serializable]
    public class AzureDataService
    {
        private static readonly string QueryString;

        public MobileServiceClient MobileClient { get; set; } = null;
        IMobileServiceSyncTable<BankAccount> accountTable;

        public async Task Initialize()
        {
            if (MobileClient?.SyncContext?.IsInitialized ?? false)
                return;

            var appUrl = "https://contosobankchatbotmobileapp.azurewebsites.net";



            MobileClient = new MobileServiceClient(appUrl);

            //InitialzeDatabase for path
            var path = InitializeDatabase();
            path = Path.Combine(MobileServiceClient.DefaultDatabasePath, path);

            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);

            //Define table
            store.DefineTable<BankAccount>();

            //Initialize SyncContext
            await MobileClient.SyncContext.InitializeAsync(store);

            accountTable = MobileClient.GetSyncTable<BankAccount>();
        }

        private string InitializeDatabase()
        {
#if __ANDROID__ || __IOS__
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
#endif
            SQLitePCL.Batteries.Init();

            var path = "syncstore.db";

#if __ANDROID__
            path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), path);

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
#endif

            return path;
        }

        public async Task<SearchResult> SearchByName(string name)
        {
            using (var httpClient = new HttpClient())
            {
                string nameQuery = $"{QueryString}search={name}";
                string response = await httpClient.GetStringAsync(nameQuery);
                return JsonConvert.DeserializeObject<SearchResult>(response);
            }
        }

        public async Task SyncAccount()
        {
            try
            {

                await accountTable.PullAsync("allAccount", accountTable.CreateQuery());

                await MobileClient.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to sync accounts, that is alright as we have offline capabilities: " + ex);
            }

        }

        public async void AddBankAccount(BankAccount account)
        {
            //Initialize();
            
            //accountTable.InsertAsync(account);

            //await SyncAccount();
            
            //return account;
        }

    }
}