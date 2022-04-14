using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TePass.Models;
using TePass.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TePass.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionIn : ContentPage
    {
        public Test Test { get; private set; }
        public TestListViewModel ViewModel { get; private set; }
        public QuestionIn(Test model, TestListViewModel viewModel)
        {
            InitializeComponent();
            Test = model;
            ViewModel = viewModel;
            BindingContext = this;
            viewModel.QuestionIn = this;
        }
        protected override void OnAppearing()
        {
            CheckLang();
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                ViewModel.SelectedQuestion = null;
                ViewModel.SelectedVarient = null;
                ViewModel.IsNoConnection = false;
                base.OnAppearing();
            }
            else ViewModel.IsNoConnection = true;
        }
        private void CheckLang()
        {
            if (ViewModel.SelectedLanguage == "English")
            {
                LOADING.Text = "Loading...";
                NO_CONNECTION.Text = "No internet connection";
                ACCEPT.Text = "Accept answer";
            }
            else if (ViewModel.SelectedLanguage == "Русский")
            {
                LOADING.Text = "Загрузка...";
                NO_CONNECTION.Text = "Нет подключения";
                ACCEPT.Text = "Подтвердить";
            }
            else if (ViewModel.SelectedLanguage == "Беларуская")
            {
                LOADING.Text = "Загрузка...";
                NO_CONNECTION.Text = "Няма падключення";
                ACCEPT.Text = "Адказаць";
            }
        }
    }
}