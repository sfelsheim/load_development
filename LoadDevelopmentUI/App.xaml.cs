using System;
using System.IO;
using Xamarin.Forms;
using DataAccess;

namespace LoadDevelopmentUI
{
    public partial class App : Application
    {
        static LoadDatabase loadDatabase;

        public static LoadDatabase Database
        {
            get
            {
                if (loadDatabase == null)
                {
                    string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

                    string dbPath = Path.Combine(libFolder, "master.db");
                    loadDatabase = new LoadDatabase(dbPath);
                }
                return loadDatabase;
            }
        }

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjQyMTk2QDMxMzgyZTMxMmUzMERvQnoxeS9sRVJZZjk5U2pvRndXc1UzOC9JcmdFYTh2NThsczJLWDd6M1k9");

            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
