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
        private Helper.ManualVariationCreator mvCreator;
        private ModelView.TestLoadPopupViewModel popUpModelView;

        public TestLoadPage(Load load)
        {
            mvCreator = new Helper.ManualVariationCreator(App.Database);
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
            this.popUpModelView = new TestLoadPopupViewModel();
            this.choicePopup.BindingContext = popUpModelView;
        }

        public void ActionPopupClosed(object sender, EventArgs e)
        {
            if (!popUpModelView.WasAccepted)
                return;

            if (popUpModelView.EditShotsChecked)
            {
                var loadStringScreen = new TestLoadString(loadBeingTested, popUpModelView.LoadBeingTested);
                Navigation.PushAsync(loadStringScreen);
            }
            else if (popUpModelView.ViewTargetImageChecked)
            {
                var capturePage = new TargetImagePage(popUpModelView.LoadBeingTested);
                Navigation.PushAsync(capturePage);
            }
        }

        void ShotStringsListView_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            if ((e.SelectedItem as LoadString).AvgVelocity > 0)
            {
                popUpModelView.LoadBeingTested = e.SelectedItem as LoadString;
                choicePopup.Show();
	        }
            else
            { 
                var loadStringScreen = new TestLoadString(loadBeingTested, e.SelectedItem as LoadString);
                Navigation.PushAsync(loadStringScreen);
	        }
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
                    SuggestionData = new VelocityNodeSuggestionData { PowderCharge = (float)d.X },
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

            Load newLoad = createLoadTemplateForSuggestion(loadBeingTested);

            // duplicate the current load and add manual variations for each
            // selected suggestion
            int numVarations = 0;
            foreach (var sug in modelView.Suggestions)
            {
                var mv = mvCreator.CreateManualVariation(newLoad.LoadID, 5, newLoad.COAL, sug.SuggestionData.PowderCharge);
                modelView.Database.InsertManualVariation(mv);
                ++numVarations;
	        }

            newLoad.ManualVariations = numVarations;
            modelView.Database.UpdateLoad(newLoad);

            DisplayAlert("Load Created", string.Format(
                "A new load named, {0} with {1} varations has been created in Develop.",
                    newLoad.Name, numVarations), "OK");

            modelView.UnSelectSuggestions();
	    }

        private Load createLoadTemplateForSuggestion(Load loadBeingTested)
        {
            Load load = modelView.Database.CreateNewLoad();

            load.Name = "Refined: " + loadBeingTested.Name;
            load.RifleID = loadBeingTested.RifleID;
            load.PowderManfID = loadBeingTested.PowderManfID;
            load.PowderModelID = loadBeingTested.PowderModelID;
            load.BulletManfID = loadBeingTested.BulletManfID;
            load.BulletModelID = loadBeingTested.BulletModelID;
            load.BulletWeightID = loadBeingTested.BulletWeightID;
            load.PrimerManfID = loadBeingTested.PrimerManfID;
            load.CaseManfID = loadBeingTested.CaseManfID;
            load.CaseOAL = loadBeingTested.CaseOAL;
            load.CaseHeadspace = loadBeingTested.CaseHeadspace;
            load.VaryByPowderCharge = false;
            load.VaryByCOAL = false;
            load.VaryManually = true;
            // setup number of manual variations later
            load.ShotsPerVariation = loadBeingTested.ShotsPerVariation;
            load.PowderVariationAmount = loadBeingTested.PowderVariationAmount;
            load.StartingPowderCharge = loadBeingTested.StartingPowderCharge;
            load.COAL = loadBeingTested.COAL;
            load.COALVariationAmount = loadBeingTested.COALVariationAmount;
            load.StartingCOAL = loadBeingTested.StartingCOAL;
            load.PowderCharge = loadBeingTested.PowderCharge;
            load.BulletWeightID = loadBeingTested.BulletWeightID;
            load.LoadState = Load.DEVELOP_STATE;
            load.FowlingRounds = loadBeingTested.FowlingRounds;
            load.IsNew = loadBeingTested.IsNew;

            return load;
        }

        void actionRadioGroup_CheckedChanged(System.Object sender, Syncfusion.XForms.Buttons.CheckedChangedEventArgs e)
        {
        }
    }
}
