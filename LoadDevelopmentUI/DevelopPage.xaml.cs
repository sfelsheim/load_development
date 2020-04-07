using System;
using System.Collections.Generic;
using DataAccess.Model;
using Xamarin.Forms;

namespace LoadDevelopmentUI
{
    public partial class DevelopPage : ContentPage
    {
        private ModelView.DevelopModelView modelView;

        public DevelopPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            modelView = new ModelView.DevelopModelView(App.Database);
            this.BindingContext = modelView;
        }

        async void NewLoadButtonClicked(object sender, EventArgs e)
        {
            Load newLoad = App.Database.CreateNewLoad();
            var page = new NewLoadPage(newLoad);

            await Navigation.PushAsync(page);
        }

        void OnCopyLoadMenuItemClicked(object sender, EventArgs e)
        {
            var mi = sender as MenuItem;
            LoadDisplay currentLoad = mi.CommandParameter as LoadDisplay;

            modelView.MakeCopyOfLoad(currentLoad.LoadID);
            modelView.Loads = modelView.GetDisplayLoads();
		}

        async void OnDevelopLoadsSelectionChanged(object sender, EventArgs e)
        {
            var currentLoadDisplay = developCollectionView.SelectedItem as LoadDisplay;

            Load daLoad = modelView.GetLoad(currentLoadDisplay.LoadID);

            var page = new NewLoadPage(daLoad);

            await Navigation.PushAsync(page);
        }

        void OnDeleteMenuItemClicked(object sender, EventArgs e)
        {
            var mi = sender as MenuItem;
            LoadDisplay currentLoad = mi.CommandParameter as LoadDisplay;

            modelView.DeleteLoad(currentLoad.LoadID);
            modelView.Loads = modelView.GetDisplayLoads();
		}
    }
}
