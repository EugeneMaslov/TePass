using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TePass.Views
{
    public partial class App : Application
    {
        public const string HEADER = "TePass";
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
