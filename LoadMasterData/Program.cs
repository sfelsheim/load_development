using System;
using DataAccess.Model;
using SQLite;

namespace LoadMasterData
{
    class Program
    {
        static void Main(string[] args)
        {
            LoadDevelopmentDatabase db = new LoadDevelopmentDatabase(
                @"/Users/steve.felsheim/Projects/load_development/LoadMasterData/Database/master.db");

            //db.TestLoad();

            db.LoadCaliberMaster(
                new System.IO.StreamReader(
                    @"/Users/steve.felsheim/Projects/load_development/LoadMasterData/MasterData/CaliberMaster.csv"));


            db.LoadPowderManufacturerMaster(new System.IO.StreamReader(
                @"/Users/steve.felsheim/Projects/load_development/LoadMasterData/MasterData/PowderManufacturerMaster.csv"));
            db.LoadPowderMaster(new System.IO.StreamReader(
                @"/Users/steve.felsheim/Projects/load_development/LoadMasterData/MasterData/PowderMaster.csv"));

            db.LoadBulletMaster(@"/Users/steve.felsheim/Projects/load_development/LoadMasterData/MasterData/bulletMaster.db",
                @"/Users/steve.felsheim/Projects/load_development/LoadMasterData/Database/master.db");

            db.LoadPrimerManufacturerMaster(new System.IO.StreamReader(
                @"/Users/steve.felsheim/Projects/load_development/LoadMasterData/MasterData/primers.csv"));

            db.LoadPrimerModelMaster(new System.IO.StreamReader(
                @"/Users/steve.felsheim/Projects/load_development/LoadMasterData/MasterData/primerModels.csv"));

            db.LoadBrassMaster(new System.IO.StreamReader(
                @"/Users/steve.felsheim/Projects/load_development/LoadMasterData/MasterData/brass.csv"));

            db.CreateDisplayView();
        }



        private static void parseBulletMasterHtml()
        {
                var db = new SQLiteConnection(@"C:\Users\steve.felsheim\source\repos\PrecisionLoadDevelopment\LoadDevelopmentDataAccess\MasterData\bulletMaster.db");
                db.CreateTable<BulletMasterProfile>();

                var reader = new System.IO.StreamReader(@"C:\Users\steve.felsheim\source\repos\PrecisionLoadDevelopment\LoadDevelopmentDataAccess\MasterData\bulletmanf.html");
                string line;
                var newBullet = new BulletMasterProfile();
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("MANF"))
                    {
                        newBullet.Manufacturer = extractTarget(line, "MANF");
                    }
                    else if (line.Contains("MODEL"))
                    {
                        newBullet.Model = extractTarget(line, "MODEL");

                    }
                    else if (line.Contains("CAL"))
                    {
                        newBullet.Caliber = extractTarget(line, "CAL");
                    }
                    else if (line.Contains("GRAINS"))
                    {
                        newBullet.Weight = extractTarget(line, "GRAINS");
                    }
                    else
                        continue;

                    if (newBullet.IsReady())
                    {
                        newBullet.BulletMasterProfileID = Guid.NewGuid();
                        db.Insert(newBullet);
                        newBullet.Clear();
                    }
                }
            }

        private static string extractTarget(string line, string target)
        {
            int targetPos = line.IndexOf(target);
            int start = targetPos + target.Length + 2;
            int end = line.IndexOf('<', start);

            return line.Substring(start, end - start).Trim();
        }
    }
}
