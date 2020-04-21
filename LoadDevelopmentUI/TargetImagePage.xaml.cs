using System;
using System.Collections.Generic;
using System.IO;
using DataAccess.Model;
using LoadDevelopmentUI.Helper;
using Plugin.Media;
using Syncfusion.SfImageEditor.XForms;
using Xamarin.Forms;

namespace LoadDevelopmentUI
{
    public partial class TargetImagePage : ContentPage
    {
      
        private const string IMAGE_DIRECTORY = "TargetImages";

        private ModelView.CaptureImageModelView modelView;
        private LoadString shotString;
        private string imageFileName;


        public TargetImagePage(LoadString shotString)
        {
            this.shotString = shotString;
            imageFileName = shotString.LoadStringID.ToString() + ".jpg"; 
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            modelView = new ModelView.CaptureImageModelView(shotString);
            this.BindingContext = modelView;

            if (DependencyService.Get<ISavePhoto>().PhotoExists(imageFileName, IMAGE_DIRECTORY))
            {

                modelView.TargetImage = ImageSource.FromFile(
		            DependencyService.Get<ISavePhoto>().GetPhotoPath(imageFileName, IMAGE_DIRECTORY));
	        }
        }

        async public void CaptureTargetImageButtonClicked(object sender, EventArgs e)
        {
            if (DependencyService.Get<ISavePhoto>().PhotoExists(imageFileName, IMAGE_DIRECTORY))
            {
                bool answer = await DisplayAlert("Image Exits", "Replace Existing Target Image?", "Yes", "No");

                if (!answer)
                    return;

                DependencyService.Get<ISavePhoto>().DeletePhoto(imageFileName, IMAGE_DIRECTORY);
	        }

            await CrossMedia.Current.Initialize();

            var file = await CrossMedia.Current.TakePhotoAsync(
                new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small,
                    SaveToAlbum = false,
                    CompressionQuality = 92,
                    AllowCropping = false,
                    Directory = IMAGE_DIRECTORY,
                    Name = imageFileName
                });

            modelView.TargetImage = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });
        }

        void SelectTargetImageButtonClicked(object sender, EventArgs args)
        { 
	    }

        void TargetImageEditorIsSaving(object sender, ImageSavingEventArgs args)
        {
            args.Cancel = true;
            DependencyService.Get<ISavePhoto>().SavePhoto(imageFileName, 
		        IMAGE_DIRECTORY, args.Stream);
	    }
    }

}
