using System;
using System.ComponentModel;

namespace LoadDevelopmentUI.ModelView
{
    public class VelocityNodeSuggestionData
    { 
        public float PowderCharge { get; set; }
    }

    public class Suggestion : INotifyPropertyChanged
    {
        private bool isChecked = false;
        public string Display { get; set; }
        public string Detail { get; set; }
        public bool ShowSelect { get; } = true;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (value != isChecked)
                {
                    isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
                }
            }
        }

        public VelocityNodeSuggestionData SuggestionData { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
