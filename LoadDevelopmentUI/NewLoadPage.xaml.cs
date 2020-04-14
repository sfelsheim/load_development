using System;
using System.Collections.Generic;
using DataAccess.Model;
using Xamarin.Forms;

namespace LoadDevelopmentUI
{
    public partial class NewLoadPage : ContentPage
    {
        private Helper.ManualVariationCreator mvCreator;
        private ModelView.LoadModelView modelView;
        private decimal currentRifleCaliber = -1.0M;
        private Load currentLoad;
        private List<LoadString> loadRecipe;
        private enum VariationMode
        {
            Powder,
            COAL,
            Manual
        };

        public NewLoadPage(Load load)
        {
            InitializeComponent();
            currentLoad = load;
            mvCreator = new Helper.ManualVariationCreator(App.Database);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            modelView = new ModelView.LoadModelView(App.Database, currentLoad);
            this.BindingContext = modelView;

            generateLoadRecipe();
        }

        public void OnVariationUnFocused(object sender, FocusEventArgs e)
        {
            generateLoadRecipe();
		}

        public void generateLoadRecipe()
        {
	        loadRecipe = new List<LoadString>();
            if (!currentLoad.VaryManually)
            {
                var variations = currentLoad.VaryByPowderCharge
                                    ? currentLoad.PowderVariations
                                    : currentLoad.CoalVariations;

                float coalAccum = 0F;
                float powderChargeAccum = 0F;
                for (int i = 0; i < variations; ++i)
                {
                    LoadString shotString = new LoadString();
                    shotString.LoadID = currentLoad.LoadID;
                    shotString.LoadStringID = Guid.NewGuid();
                    shotString.ID = (i + 1).ToString();
                    shotString.NumRounds = currentLoad.ShotsPerVariation;
                    if (currentLoad.VaryByPowderCharge)
                    {
                        shotString.Variation = VariationType.ByPowder;
                        powderChargeAccum += i == 0 ? currentLoad.StartingPowderCharge
                                                         : currentLoad.PowderVariationAmount;
                        shotString.PowderCharge = powderChargeAccum;

                        shotString.Coal = currentLoad.COAL;
                    }
                    else
                    {
                        shotString.Variation = VariationType.ByCoal;
                        shotString.PowderCharge = currentLoad.PowderCharge;
                        coalAccum += i == 0 ? currentLoad.StartingCOAL
                                                 : currentLoad.COALVariationAmount;
                        shotString.Coal = coalAccum;
                    }

                    loadRecipe.Add(shotString);
                }
            }
            else
            {
                var manuals = modelView.GetManualVariations(currentLoad.LoadID);
                int i = 0;
                foreach(var man in manuals)
                {
                    loadRecipe.Add(mvCreator.CreateLoadString(man, (i+1).ToString()));
                    ++i;
		        }
	        }

            modelView.LoadDescription = generateLoadDescription();
            modelView.LoadStrings = loadRecipe;
		}

        private string generateLoadDescription()
        {
            return generateBulletDescription() + " using " + generatePowderDescription();
		}

        private string generateBulletDescription()
        {
			if (currentLoad.BulletManfID != Guid.Empty && currentLoad.BulletModelID != Guid.Empty
					&& currentLoad.BulletWeightID != Guid.Empty)
            {
                return string.Format("{0}gr {1}, {2}",
                    modelView.GetBullet(currentLoad.BulletWeightID).Weight,
                    modelView.GetBulletManf(currentLoad.BulletManfID).Name,
                    modelView.GetBullet(currentLoad.BulletModelID).Name);
			}

            return "No Bullet Selected!";
        }

        private string generatePowderDescription()
        {
            if (currentLoad.PowderModelID != Guid.Empty && currentLoad.PowderManfID != Guid.Empty)
            {
                return string.Format("{0}, {1}",
                    modelView.GetPowderManf(currentLoad.PowderManfID).Name,
                    modelView.GetPowderModel(currentLoad.PowderModelID).Name);
            }

            return "No Powder Selected!";
        }

        void OnReadyToTestClicked(object sender, EventArgs e)
        {
            modelView.SaveLoadRecipe(loadRecipe);
            modelView.SetCurrentLoadToTest();
            Navigation.PopAsync();
		}

        void LoadRecipeDeleteButtonClicked(object sender, EventArgs e)
        {
            if (currentLoad.VaryManually)
            {
                var loadStrings = modelView.LoadStrings;
                foreach (var item in loadBlockCollectionView.SelectedItems)
                {
                    modelView.DeleteManualVariation((item as LoadString).LoadStringID);
                }
            }
            generateLoadRecipe();
	    }

        void AddManualVariationButtonClicked(object sender, EventArgs e)
        {
            int numRounds;
            bool ok = int.TryParse(this.manualNumRoundsEntry.Text, out numRounds);
            if (!ok)
            {
                DisplayAlert("Invalid Entry", "You must enter a value for Number of Rounds", "OK");
                return;
            }

            float coal;
            ok = float.TryParse(this.manualCoalEntry.Text, out coal);
            if (!ok)
            {
                DisplayAlert("Invalid Entry", "You must enter a value for COAL", "OK");
                return;
            }

            float powderCharge;
            ok = float.TryParse(this.manualPowderChargeEntry.Text, out powderCharge);
            if (!ok)
            {
                DisplayAlert("Invalid Entry", "You must enter a value for Powder Charge", "OK");
                return;
            }

            modelView.AddManualVariation(currentLoad.LoadID, numRounds, coal, powderCharge);
            generateLoadRecipe();
	    }

        #region On New Navigation
        async void OnNewRifleButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewRiflePage(modelView));
        }

        async void OnNewPowderManfButtonClicked(object sender, EventArgs e)
        {

        }

        async void OnNewPowderButtonClicked(object sender, EventArgs e)
        {

        }

        async void OnNewBulletManfButtonClicked(object sender, EventArgs e)
        {

        }

        async void OnNewBulletWeightButtonClicked(object sender, EventArgs e)
        {

        }

        async void OnNewBulletButtonClicked(object sender, EventArgs e)
        {

        }

        #endregion

        #region Selections Change
        void OnBulletSelectionChanged(object sender, EventArgs e)
        {
            var selectedBulletModel = bulletPicker.SelectedItem as BulletModelMaster;
            modelView.BulletWeights = modelView.GetBulletWeights(
                selectedBulletModel.Name,
                modelView.SelectedBulletManf.BulletManufacturerMasterID,
                currentRifleCaliber);

            bulletWeightPicker.IsEnabled = true;
            newBulletWeightButton.IsEnabled = true;
        }

        void OnBulletWeightSelected(object sender, EventArgs e)
        { 
            generateLoadRecipe();
		}

        void OnPowderModelSelected(object sender, EventArgs e)
        { 
            generateLoadRecipe();
		}

        void OnPrimerManfSelectionChanged(object sender, EventArgs e)
        {
            var selectedPrimerManf = primerManfPicker.SelectedItem as PrimerManufacturerMaster;
            modelView.PrimerModel = modelView.GetPrimerModels(selectedPrimerManf.PrimerManufacturerId);

            primerPicker.IsEnabled = true;
            newPrimerButton.IsEnabled = true;
        }

        void OnBulletManfSelectionChanged(object sender, EventArgs e)
        {
            modelView.BulletModel = modelView.GetBulletModels(
                modelView.SelectedBulletManf.BulletManufacturerMasterID,
                currentRifleCaliber);

            bulletPicker.IsEnabled = true;
            newBulletButton.IsEnabled = true;
        }

        void OnPowderManfSelectionChanged(object sender, EventArgs e)
        {
            var pm = powderManfPicker.SelectedItem as PowderManufacturerMaster;
            modelView.PowderModel = modelView.GetPowderModels(
				pm.PowderManufacturerMasterID);

            powderPicker.IsEnabled = true;
            newPowderButton.IsEnabled = true;
        }

        void OnRifleSelectionChanged(object sender, EventArgs e)
        {

            Rifle currentRifle = (riflePicker.SelectedItem as Rifle);

            if (currentRifle == null)
                return;

            currentRifleCaliber = modelView.GetCaliber(currentRifle.CaliberID).BulletDiameter;

            bulletManfPicker.IsEnabled = true;
            newBulletManfButton.IsEnabled = true;
        }

        void PowderChargeSwitchToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                coalSwitch.IsToggled = false;
                manualSwitch.IsToggled = false;
                deleteManualVariationButton.IsEnabled = false;
                setupDisplayForVariation(VariationMode.Powder);
	        }
        }

        void CoalSwitchToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                powderChargeSwitch.IsToggled = false;
                deleteManualVariationButton.IsEnabled = false;
                manualSwitch.IsToggled = false;

                setupDisplayForVariation(VariationMode.COAL);
	        }
        }

        void ManualSwitchToggled(object sender, ToggledEventArgs e)
        { 
            if (e.Value)
            {
                powderChargeSwitch.IsToggled = false;
                coalSwitch.IsToggled = false;
                deleteManualVariationButton.IsEnabled = true;

                setupDisplayForVariation(VariationMode.Manual);
	        }
	    }

        #endregion

        void OnLoadNameCompleted(object sender, EventArgs e)
        {
            string val = (sender as Entry).Text;
            if (!string.IsNullOrEmpty(val) && !string.IsNullOrWhiteSpace(val))
                toggleEnableLoadNameDependentControls(true);
        }

        #region Private Methods

        private void toggleEnableLoadNameDependentControls(bool enabled)
        {
            riflePicker.IsEnabled = enabled;
            newRifleButton.IsEnabled = enabled;

            powderManfPicker.IsEnabled = enabled;
            newPowderManfButton.IsEnabled = enabled;

            primerManfPicker.IsEnabled = enabled;
            newPrimerManfButton.IsEnabled = enabled;
            caseManfPicker.IsEnabled = enabled;
            newCaseManfButton.IsEnabled = enabled;
            variationAmountEntry.IsEnabled = enabled;
            shotsPerVariationEntry.IsEnabled = enabled;
            numVariationsEntry.IsEnabled = enabled;
            startingEntry.IsEnabled = enabled;
            coalEntry.IsEnabled = enabled;
            caseOalEntry.IsEnabled = enabled;
            caseHeadspaceEntry.IsEnabled = enabled;
        }

        private void setupDisplayForVariation(VariationMode mode)
        {
            this.variaionLabel.Text = string.Format("{0} Variations", mode);
            this.variationAmountLabel.Text = string.Format("{0} Variation amount", mode);

            if (mode == VariationMode.COAL)
            {
                variationStackLayout.IsVisible = true;
                this.variationAmountEntry.IsEnabled = true;
                this.shotsPerVariationEntry.IsEnabled = true;
                this.numVariationsEntry.IsEnabled = true;

                this.variationAmountEntry.Text = "0.002";
                this.startingEntry.Placeholder = "inch";
                this.coalLabel.TextColor = Color.LightGray;
                this.coalEntry.IsEnabled = false;
                this.startingLabel.Text = string.Format("Starting COAL");
                this.powderChargeEntry.IsEnabled = true;
                this.startingEntry.IsEnabled = true;
                this.powderChargeLabel.TextColor = Color.Black;
                manualVariationStackLayout.IsVisible = false;
            }
            else if (mode == VariationMode.Powder)
            {
                variationStackLayout.IsVisible = true;
                this.variationAmountEntry.IsEnabled = true;
                this.shotsPerVariationEntry.IsEnabled = true;
                this.numVariationsEntry.IsEnabled = true;
                this.startingEntry.IsEnabled = true;

                this.variationAmountEntry.Text = "0.3";
                this.startingEntry.Placeholder = "gr";
                this.coalLabel.TextColor = Color.Black;
                this.coalEntry.IsEnabled = true;
                this.startingLabel.Text = string.Format("Starting Powder Charge");
                this.powderChargeEntry.IsEnabled = false;
                this.powderChargeLabel.TextColor = Color.LightGray;
                manualVariationStackLayout.IsVisible = false;
            }
            else // VariationMode.Manual
            {
                variationStackLayout.IsVisible = false;
                manualVariationStackLayout.IsVisible = true;
            }
	        generateLoadRecipe();
        }

        #endregion

    }

}
