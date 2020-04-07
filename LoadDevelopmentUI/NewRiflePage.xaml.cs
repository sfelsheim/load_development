using System;
using DataAccess.Model;
using Xamarin.Forms;

namespace LoadDevelopmentUI
{
    public partial class NewRiflePage : ContentPage
    {
        public NewRiflePage(ModelView.LoadModelView loadModelView)
        {
            InitializeComponent();
            this.BindingContext = loadModelView;
        }

        public Rifle NewRifle { get; private set; }

        async void OnClickSaveNewRifle(object sender, EventArgs e)
        {
            var modelView = (ModelView.LoadModelView)this.BindingContext;


            var newRifle = new Rifle();
            newRifle.CaliberID = ((CaliberMaster)this.CaliberPicker.SelectedItem).CaliberID;
            newRifle.Name = this.NameEntry.Text;
            newRifle.ID = Guid.NewGuid();
            this.NewRifle = newRifle;

            modelView.SaveRifle(newRifle);
            await Navigation.PopAsync();
        }

        void OnNameTextChanged(object sender, TextChangedEventArgs e)
        {
            this.EnableSaveButton();
        }

        void OnCaliberSelectedItem(object sender, EventArgs e)
        {
            this.EnableSaveButton();
        }

        private void EnableSaveButton()
        {
            var value = this.NameEntry.Text;
            var selectedIndex = this.CaliberPicker.SelectedIndex;

            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && selectedIndex != -1)
                this.SaveButton.IsEnabled = true;
            else
                this.SaveButton.IsEnabled = false;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var mv = this.BindingContext as ModelView.LoadModelView;

            if (mv.SelectedCaliber != null)
                this.CaliberPicker.SelectedItem = mv.SelectedCaliber;
        }

        async void OnNewCaliberButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewCaliberPage(this.BindingContext as ModelView.LoadModelView));
        }
    }
}
