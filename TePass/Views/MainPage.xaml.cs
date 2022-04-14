using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TePass.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TePass.Views
{
    public partial class MainPage : ContentPage
    {
        public TestListViewModel ViewModel { get; private set; }
        public MainPage(TestListViewModel viewModel)
        {
            InitializeComponent();
            if (viewModel == null)
            {
                ViewModel = new TestListViewModel() { Navigation = this.Navigation };
            }
            else ViewModel = viewModel;
            this.BindingContext = ViewModel;
        }
        protected override void OnAppearing()
        {
            CheckLang();
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                ViewModel.SelectedTest = null;
                ViewModel.IsNoConnection = false;
                base.OnAppearing();
            }
            else ViewModel.IsNoConnection = true;
        }
        private void CheckLang()
        {
            if (ViewModel.SelectedLanguage == "English")
            {
                IDENTIFY.Text = "Identification name: ";
                TEST_KEY.Text = "Test key: ";
                LOADING.Text = "Loading...";
                NO_CONNECTION.Text = "No internet connection";
                TEST_NOT_FOUND.Text = "Test not found";
                PASS_TEST.Text = "Pass the test";
                CHANGE_LANGUAGE.Text = "Change language";
            }
            else if (ViewModel.SelectedLanguage == "Русский")
            {
                IDENTIFY.Text = "Имя: ";
                TEST_KEY.Text = "Код теста: ";
                LOADING.Text = "Загрузка...";
                NO_CONNECTION.Text = "Нет подключения";
                TEST_NOT_FOUND.Text = "Тест не найден";
                PASS_TEST.Text = "Пройти тест";
                CHANGE_LANGUAGE.Text = "Сменить язык";
            }
            else if (ViewModel.SelectedLanguage == "Беларуская")
            {
                IDENTIFY.Text = "Імя: ";
                TEST_KEY.Text = "Код тэсту: ";
                LOADING.Text = "Загрузка...";
                NO_CONNECTION.Text = "Няма падключення";
                TEST_NOT_FOUND.Text = "Тэст не знайдзен";
                PASS_TEST.Text = "Прайсці";
                CHANGE_LANGUAGE.Text = "Змяніць мову";
            }
        }
    }
}
