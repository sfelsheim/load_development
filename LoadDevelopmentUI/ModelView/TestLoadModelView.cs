using System;
using System.Collections.Generic;
using System.ComponentModel;
using DataAccess;
using DataAccess.Model;

namespace LoadDevelopmentUI.ModelView
{
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

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class TestLoadModelView : INotifyPropertyChanged
    {
        private LoadDatabase database;
        private List<LoadString> shotStrings = new List<LoadString>();
        private LoadDisplay loadDisplay;
        private List<Suggestion> suggestions = new List<Suggestion>();


        private Load currentLoad;

        public TestLoadModelView(LoadDatabase database, Load currentLoad)
        {
            this.database = database;
            this.currentLoad = currentLoad;
            shotStrings = database.GetLoadStrings(currentLoad.LoadID);
            loadDisplay = database.GetDisplayLoad(currentLoad.LoadID);
            foreach (var val in shotStrings)
                val.VaryByPowderCharge = loadDisplay.VaryByPowderCharge;
        }

        public LoadDatabase Database { get { return database;  } }

        public string LoadTitle
        { 
            get { return loadDisplay.ListTitle;  }
		}

        public string BulletLine
        { 
            get { return loadDisplay.BulletLabel;  }
		}

        public string PowderLine
        { 
            get { return loadDisplay.PowderLabel;  }
		}

        public List<LoadString> ShotStrings
        { 
            get { return shotStrings; }

            set
            { 
                if (value != shotStrings)
                {
                    shotStrings = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShotStrings"));
                }
			}
		}

        public List<Suggestion> Suggestions 
        {
            get { return suggestions; }

            set
            {
                if (value != suggestions)
                {
                    suggestions = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Suggestions"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool HasSuggestionsSelected()
        {
            foreach (var sug in Suggestions)
            {
                if (sug.IsChecked)
                    return true;
	        }

            return false;
        }
    }
}
