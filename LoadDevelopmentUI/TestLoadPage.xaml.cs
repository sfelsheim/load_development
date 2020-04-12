using System;
using System.Collections.Generic;
using DataAccess.Model;
using LoadDevelopmentUI.ModelView;
using OxyPlot;
using Xamarin.Forms;

namespace LoadDevelopmentUI
{
    public partial class TestLoadPage : ContentPage
    {
        private Load loadBeingTested;
        private ModelView.TestLoadModelView modelView;

        public TestLoadPage(Load load)
        {
            loadBeingTested = load;

            InitializeComponent();

            this.ShotStringsListView.ItemSelected += ShotStringsListView_ItemSelected;
            this.ShotStringButton.IsEnabled = false;
            this.ShotStringsListView.SelectionMode = ListViewSelectionMode.Single;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.modelView = new ModelView.TestLoadModelView(App.Database, loadBeingTested);
            this.BindingContext = modelView;
            this.ShotStringsListView.ItemsSource = modelView.ShotStrings;
            this.ShotStringButton.IsEnabled = false;
            this.SuggestionsButton.IsEnabled = true;
            this.BuildLoadFromSuggestionsButton.IsVisible = false;
        }

        async void ShotStringsListView_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            var loadStringScreen = new TestLoadString(loadBeingTested, e.SelectedItem as LoadString);
            await Navigation.PushAsync(loadStringScreen);
        }

        async void ViewVelocityGraphToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new TestPlot(modelView)); 
        }

        void ShotStringButton_Clicked(object sender, EventArgs e)
        {
            this.ShotStringsListView.ItemSelected += ShotStringsListView_ItemSelected;
            this.ShotStringsListView.SelectionMode = ListViewSelectionMode.Single;

            this.ShotStringButton.IsEnabled = false;
            this.SuggestionsButton.IsEnabled = true;

            this.ShotStringsListView.ItemsSource = modelView.ShotStrings;
            this.BuildLoadFromSuggestionsButton.IsVisible = false;
	    }

        void SuggestionsButton_Clicked(object sender, EventArgs e)
        {
            this.ShotStringsListView.ItemSelected -= ShotStringsListView_ItemSelected;
            this.ShotStringsListView.SelectionMode = ListViewSelectionMode.None;

            this.ShotStringButton.IsEnabled = true;
            this.SuggestionsButton.IsEnabled = false;

            var nodeDataPoints = Helper.VelocityNodeCalc.CalculateVelocityNodes(modelView.ShotStrings);
            var suggestions = new List<Suggestion>();
            foreach(DataPoint d in nodeDataPoints)
            {
                suggestions.Add(new Suggestion 
		        { 
		            Display = string.Format("New Load at {0:F1} gr", d.X), 
		            Detail = "Suggested Velocity Node" 
		        });
	        }
            modelView.Suggestions = suggestions;
            this.ShotStringsListView.ItemsSource = modelView.Suggestions;
            this.BuildLoadFromSuggestionsButton.IsVisible = true;
        }

        void BuildLoadFromSuggestionsButton_Clicked(object sender, EventArgs e)
        {
            if (!modelView.HasSuggestionsSelected())
            {
                DisplayAlert("No Selections", "Please select suggestions from the list to build a new load.",
                    "OK");
                return;
            }

            foreach (var sug in modelView.Suggestions)
            {

	        }
	    }
    }
}
