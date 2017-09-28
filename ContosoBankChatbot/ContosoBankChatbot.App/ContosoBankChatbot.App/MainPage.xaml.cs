using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ContosoBankChatbot.App
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var browser = new WebView();

            browser.Source = "https://webchat.botframework.com/embed/ScottLuoContosoBankChatbot?s=0qmvQRyynHI.cwA.zQg.Yrq8EFiztU5T0r_ExIbn7cIWGuzs-S9-CQO71z7oCxM";

            this.Content = browser;
        }
    }
}
