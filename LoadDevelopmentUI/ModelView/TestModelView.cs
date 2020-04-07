using System;
using System.Collections.Generic;
using System.ComponentModel;
using DataAccess;
using DataAccess.Model;

namespace LoadDevelopmentUI.ModelView
{
    public class TestModelView : DevelopModelView, INotifyPropertyChanged 
    {
        private List<LoadDisplay> loadsToTest = new List<LoadDisplay>();

        private LoadDatabase database;


        public TestModelView(LoadDatabase loadDatabase) : base(loadDatabase)
        {
            database = loadDatabase;
            loadsToTest = database.GetDisplayLoads(Load.TEST_STATE);
        }

        public List<LoadDisplay> LoadsToTest
        {
            get { return loadsToTest; }
            set
            {
                if (loadsToTest!= value)
                {
                    loadsToTest = value;
                    PropertyChangedTest?.Invoke(this, new PropertyChangedEventArgs("LoadsToTest"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChangedTest;
    }
}
