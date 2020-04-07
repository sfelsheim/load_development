using System;
using System.Collections.Generic;
using DataAccess.Model;
using Xamarin.Forms;

namespace LoadDevelopmentUI
{
    public partial class NewLoadPage : ContentPage
    {
        private ModelView.LoadModelView modelView;
        private decimal currentRifleCaliber = -1.0M;
        private Load currentLoad;
        private List<LoadString> loadRecipe;
        private enum VariationMode
        {
            Powder,
            COAL
        };

        public NewLoadPage(Load load)
        {
            InitializeComponent();
            currentLoad = load;

            modelView = new ModelView.LoadModelView(App.Database, currentLoad);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.BindingContext = modelView;

			populateControls();
        }

        public void OnVariationUnFocused(object sender, FocusEventArgs e)
        {
            generateLoadRecipe();
		}

        public void generateLoadRecipe()
        {
            loadRecipe = new List<LoadString>();
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
                    powderChargeAccum += i == 0 ? currentLoad.StartingPowderCharge
                                                     : currentLoad.PowderVariationAmount;
                    shotString.PowderCharge = powderChargeAccum;

                    shotString.Coal = currentLoad.COAL;
                }
                else
                {
                    shotString.PowderCharge = currentLoad.PowderCharge;
                    coalAccum += i == 0 ? currentLoad.StartingCOAL
                                             : currentLoad.COALVariationAmount;
                    shotString.Coal = coalAccum;
                }

                loadRecipe.Add(shotString);
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
            coalSwitch.IsToggled = !e.Value;
            if (e.Value)
            {
                setupDisplayForVariation(VariationMode.Powder);
            }
        }

        void CoalSwitchToggled(object sender, ToggledEventArgs e)
        {
            powderChargeSwitch.IsToggled = !e.Value;
            if (e.Value)
                setupDisplayForVariation(VariationMode.COAL);
        }


        #endregion

        void OnLoadNameCompleted(object sender, EventArgs e)
        {
            string val = (sender as Entry).Text;
            if (!string.IsNullOrEmpty(val) && !string.IsNullOrWhiteSpace(val))
            {
                toggleEnableLoadNameDependentControls(true);
            }
        }

        #region Private Methods
        private void populateControls()
        {
            if (!string.IsNullOrEmpty(currentLoad.Name))
            {
                modelView.LoadName = currentLoad.Name;
                toggleEnableLoadNameDependentControls(true);
            }

            if (currentLoad.RifleID != Guid.Empty)
            {
                riflePicker.SelectedItem = modelView.GetRifle(currentLoad.RifleID);
            }

            if (currentLoad.PowderManfID != Guid.Empty)
            {
                modelView.SelectedPowderManf = modelView.GetPowderManf(currentLoad.PowderManfID);
            }

            if (currentLoad.PowderModelID != Guid.Empty)
            {
                modelView.SelectedPowderModel = modelView.GetPowderModel(currentLoad.PowderModelID);
            }

            if (currentLoad.BulletManfID != Guid.Empty)
            {
                modelView.SelectedBulletManf = modelView.GetBulletManf(currentLoad.BulletManfID);
            }

            if (currentLoad.BulletModelID != Guid.Empty)
            {
                var bullet = modelView.GetBullet(currentLoad.BulletModelID);

                modelView.SelectedBullet = bullet;

                modelView.BulletWeights = modelView.GetBulletWeights(
                    bullet.Name,
                    bullet.BulletManufacturerID,
                    bullet.Diameter);
            }

            if (currentLoad.BulletWeightID != Guid.Empty)
            {
                var bullet = modelView.GetBullet(currentLoad.BulletWeightID);
                modelView.SelectedBulletWeight = bullet;
            }
		   
            if (currentLoad.PrimerManfID != Guid.Empty)
            {
                modelView.SelectedPrimerManf = modelView.GetPrimerManufacturer(currentLoad.PrimerManfID);
            }

            if (currentLoad.PrimerModelID != Guid.Empty)
                modelView.SelectedPrimerModel = modelView.GetPrimerModel(currentLoad.PrimerModelID);

            if (currentLoad.CaseManfID != Guid.Empty)
                modelView.SelectedBrass = modelView.GetBrassMaster(currentLoad.CaseManfID);

            modelView.SelectedIsVaryByPowderCharge = currentLoad.VaryByPowderCharge;
            modelView.SelectedIsVaryByCoal = currentLoad.VaryByCOAL;

            if (currentLoad.ShotsPerVariation > 0)
                modelView.SelectedShotsPerVariation = currentLoad.ShotsPerVariation.ToString();

            if (currentLoad.VaryByPowderCharge)
            {
                if (currentLoad.PowderVariations > 0)
                    modelView.SelectedVariations = currentLoad.PowderVariations.ToString();

                if (currentLoad.PowderVariationAmount > 0)
                    modelView.SelectedVariationAmount = currentLoad.PowderVariationAmount.ToString("F1");

                if (currentLoad.StartingPowderCharge > 0)
                    modelView.SelectedStarting = currentLoad.StartingPowderCharge.ToString("F1");
				
				if (currentLoad.COAL > 0)
					modelView.SelectedCoal = currentLoad.COAL.ToString("F3");
			}
            else
            { 
                if (currentLoad.CoalVariations > 0)
                    modelView.SelectedVariations = currentLoad.CoalVariations.ToString();

                if (currentLoad.COALVariationAmount > 0)
                    modelView.SelectedVariationAmount = currentLoad.COALVariationAmount.ToString("F3");

                if (currentLoad.StartingCOAL > 0)
                    modelView.SelectedStarting = currentLoad.StartingCOAL.ToString("F3");
            
				if (currentLoad.PowderCharge > 0)
					modelView.SelectedPowderCharge = currentLoad.StartingPowderCharge.ToString("F1");
			}

            generateLoadRecipe();

        }

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
                this.variationAmountEntry.Text = "0.002";
                this.startingEntry.Placeholder = "inch";
                this.coalLabel.TextColor = Color.LightGray;
                this.coalEntry.IsEnabled = false;
                this.startingLabel.Text = string.Format("Starting COAL");
                this.powderChargeEntry.IsEnabled = true;
                this.powderChargeLabel.TextColor = Color.Black;
            }
            else
            {
                this.variationAmountEntry.Text = "0.3";
                this.startingEntry.Placeholder = "gr";
                this.coalLabel.TextColor = Color.Black;
                this.coalEntry.IsEnabled = true;
                this.startingLabel.Text = string.Format("Starting Powder Charge");
                this.powderChargeEntry.IsEnabled = false;
                this.powderChargeLabel.TextColor = Color.LightGray;
            }
        }

        #endregion

    }

}
