using System;
using DataAccess.Model;
using Plugin.Media;
using Xamarin.Forms;

namespace LoadDevelopmentUI
{
    public partial class TestLoadString : ContentPage
    {
        private ModelView.TestLoadShotStringModelView modelView;
        private Load loadBeingTested;
        private LoadString loadString;

        public TestLoadString(Load load, LoadString loadString)
        {
            this.loadString = loadString;
            this.loadBeingTested = load;

            InitializeComponent();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.modelView = new ModelView.TestLoadShotStringModelView(App.Database, 
				loadBeingTested, loadString);

            this.BindingContext = modelView;
            modelView.UpdateStats();
        }

        void VelocityEntry_Unfocused(object sender, FocusEventArgs e)
        {
            LoadStringShot shot = ((sender as Entry).BindingContext) as LoadStringShot;

            modelView.UpdateLoadStringShot(shot);
            modelView.UpdateStats();
        }

        void CheckBox_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            LoadStringShot shot = ((sender as CheckBox).BindingContext) as LoadStringShot;

            modelView.UpdateLoadStringShot(shot);
            modelView.UpdateStats();
        }

        async void CaptureTargetButtonClicked(object sender, EventArgs e)
        {
            var capturePage = new TargetImagePage(loadString);
            await Navigation.PushAsync(capturePage);
        }
    }
}
