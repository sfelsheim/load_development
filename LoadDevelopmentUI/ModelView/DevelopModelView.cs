using System;
using System.Collections.Generic;
using System.ComponentModel;
using DataAccess;
using DataAccess.Model;

namespace LoadDevelopmentUI.ModelView
{
    public class DevelopModelView : INotifyPropertyChanged
    {
        private LoadDatabase database;
        private List<LoadDisplay> loadDisplays = new List<LoadDisplay>();

        public DevelopModelView(LoadDatabase database)
        {
            this.database = database;
            loadDisplays = database.GetDisplayLoads(Load.DEVELOP_STATE);
        }

        public List<LoadDisplay> Loads
        {
            get { return loadDisplays;  }
            set
            {
                if (loadDisplays != value)
                {
                    loadDisplays = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Loads"));
                }
            }
        }

        public Load GetLoad(Guid loadID)
        {
            return database.GetLoad(loadID);
        }

        public void DeleteLoad(Guid loadID)
        {
            database.DeleteLoad(loadID);
        }

        public void MakeCopyOfLoad(Guid loadID)
        {
            Load loadToCopy = database.GetLoad(loadID);

            Load newLoad = new Load();
            newLoad = loadToCopy;
            newLoad.LoadID = Guid.NewGuid();
            newLoad.Name = "Copy of " + loadToCopy.Name;

            database.NewLoad(newLoad);
        }

        public List<LoadDisplay> GetDisplayLoads()
        {
            return database.GetDisplayLoads(Load.DEVELOP_STATE);
		}

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
