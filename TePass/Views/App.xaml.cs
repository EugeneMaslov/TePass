using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TePass.Views
{
    public partial class App : Application
    {
        public const string HEADER = "Te";
        public const string HEADER1 = "st";
        public const string HEADER2 = "Pass";
        public const string HEADER3 = "ing";
        public const string IsTrueText = "Правильный ответ";
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
