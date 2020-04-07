using System;
using System.Collections.Generic;
using DataAccess.Model;
using Xamarin.Forms;

namespace LoadDevelopmentUI
{
    public partial class TestPage : ContentPage
    {
        private ModelView.TestModelView modelView;

        public TestPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            modelView = new ModelView.TestModelView(App.Database);

            this.BindingContext = modelView;
        }

        async public void OnTestLoadsSelectionChanged(object sender, SelectedItemChangedEventArgs args)
        {
            var currentLoadDisplay = testCollectionView.SelectedItem as LoadDisplay;

            Load daLoad = modelView.GetLoad(currentLoadDisplay.LoadID);

            var page = new TestLoadPage(daLoad);

            await Navigation.PushAsync(page);
        }

        public void OnDeleteMenuItemClicked(object sender, EventArgs args)
        { 

		}

        public void OnCopyLoadMenuItemClicked(object sender, EventArgs args)
        { 
		}
    }
}
