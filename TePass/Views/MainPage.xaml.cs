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
        public MainPage()
        {
            InitializeComponent();
            ViewModel = new TestListViewModel() { Navigation = this.Navigation };
            this.BindingContext = ViewModel;
        }
        protected override async void OnAppearing()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                isNotConnection.IsVisible = false;
                ViewModel.SelectedTest = null;
                base.OnAppearing();
            }
            else isNotConnection.IsVisible = true;
        }
    }
}
