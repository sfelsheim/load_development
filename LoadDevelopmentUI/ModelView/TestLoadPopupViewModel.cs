using System;
using System.ComponentModel;
using System.Windows.Input;
using DataAccess.Model;
using Xamarin.Forms;

namespace LoadDevelopmentUI.ModelView 
{
    public class TestLoadPopupViewModel : INotifyPropertyChanged 
    {
        private bool isOpen;
        private bool accepted = false;
        private bool editShotsChecked = true;
        private bool viewTargetImageChecked = false;
        private bool keepForUseChecked = false;

        public TestLoadPopupViewModel()
        {
            PopupAcceptCommand = new Command(PopupAccept); //CanExecute() will be call the PopupAccept method
            PopupDeclineCommand = new Command(PopupDecline); //CanExecute() will be call the PopupDecline method
            PopupCommand = new Command(Popup);
        }

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICommand PopupAcceptCommand { get; set; }
        public ICommand PopupDeclineCommand { get; set; }
        public ICommand PopupCommand { get; set; }

        public bool WasAccepted
        { 
            get { return accepted;  }
	    }

        public bool PopupOpen
        {
            get { return isOpen; }
            set
            {
                isOpen = value;
                OnPropertyChanged(nameof(PopupOpen));
            }
        }

        public bool EditShotsChecked 
	    {
            get { return editShotsChecked; }
            set
            {
                if (value != editShotsChecked)
                {
                    editShotsChecked = value;
                    OnPropertyChanged(nameof(EditShotsChecked));
                }
            } 
	    }

        public bool ViewTargetImageChecked
        { 
            get { return viewTargetImageChecked;  }
            set 
	        {
                if (value != viewTargetImageChecked)
                {
                    viewTargetImageChecked = value;
                    OnPropertyChanged(nameof(ViewTargetImageChecked));
                }
	        }
	    }
        public bool KeepForUseChecked
        { 
            get { return keepForUseChecked;  }
            set
            { 
                if (value != keepForUseChecked)
                {
                    keepForUseChecked = value;
                    OnPropertyChanged(nameof(KeepForUseChecked));
		        }
	        }
	    }

        public LoadString LoadBeingTested { get; set; }

        private void Popup()
        {
            PopupOpen = true;
        }

        private void PopupAccept()
        {
            accepted = true;
        }

        private void PopupDecline()
        {
            accepted = false;
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
