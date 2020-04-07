using System;
using System.Collections.Generic;
using DataAccess.Model;
using Xamarin.Forms;

namespace LoadDevelopmentUI
{
    public partial class NewCaliberPage : ContentPage
    {
        private ModelView.LoadModelView loadModelView;
        public NewCaliberPage(ModelView.LoadModelView loadModelView)
        {
            InitializeComponent();
            this.loadModelView = loadModelView;
        }

        public void OnClickSaveNewCaliber(object sender, EventArgs e)
        {
            var caliber = this.CaliberNameEntry.Text;
            var diameterStr = this.CaliberDiameterEntry.Text;

            decimal diameter = 0;
            try
            {
                diameter = Convert.ToDecimal(diameterStr);
            }catch(FormatException)
            {
                DisplayAlert("Invalid Bullet Diameter", 
                    "Please enter a valid, numeric bullet diameter in inches.", "OK");
                return;
            }

            var cm = new CaliberMaster();
            cm.CaliberID = Guid.NewGuid();
            cm.Name = caliber;
            cm.BulletDiameter = diameter;
            cm.UserDefined = true;
            loadModelView.SaveCaliber(cm);
            Navigation.PopAsync();
        }

        public void OnCaliberNameTextChanged(object sender, TextChangedEventArgs e)
        {
            this.EnableSaveButton();
        }

        public void OnBulletDiameterTextChanged(object sender, TextChangedEventArgs e)
        {
            this.EnableSaveButton();
        }

        private void EnableSaveButton()
        {
            var caliber = this.CaliberNameEntry.Text;
            var diameter = this.CaliberDiameterEntry.Text;

            if (!string.IsNullOrEmpty(caliber) && !string.IsNullOrWhiteSpace(caliber) 
                && !string.IsNullOrEmpty(diameter) && !string.IsNullOrWhiteSpace(diameter))
                this.SaveButton.IsEnabled = true;
            else
                this.SaveButton.IsEnabled = false;

        }
    }
}
