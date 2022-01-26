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
            this.BindingContext = this;
            ViewModel.QuestionIn = this;
        }
        protected override async void OnAppearing()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                isNotConnection.IsVisible = false;
                await ViewModel.GetQuestions();
                await ViewModel.GetVarients();
                ViewModel.SelectedQuestion = null;
                ViewModel.SelectedVarient = null;
                base.OnAppearing();
            }
            else isNotConnection.IsVisible = true;
        }
    }
}