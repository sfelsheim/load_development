using System;
using System.ComponentModel;
using System.IO;
using DataAccess;
using DataAccess.Model;
using Xamarin.Forms;

namespace LoadDevelopmentUI.ModelView
{
    public class CaptureImageModelView : INotifyPropertyChanged
    {
        private LoadDatabase database;
        private ImageSource imageSource = null;

        public CaptureImageModelView(LoadString shotString)
        {
            ShotString = shotString;
            database = App.Database;
        }

        public LoadString ShotString { get; set; }
    
        public ImageSource TargetImage
        {
            get 
	        { 
                return imageSource;
	        }
            set 
	        {
                if (value != imageSource)
                    this.imageSource = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TargetImage"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
