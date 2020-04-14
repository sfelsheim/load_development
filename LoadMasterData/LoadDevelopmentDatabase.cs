using SQLite;
using System.IO;
using System;
using DataAccess.Model;

namespace LoadMasterData
{
    public class LoadDevelopmentDatabase
    {
        readonly SQLiteAsyncConnection _database;
        string dbPath;
        public LoadDevelopmentDatabase(string dbPath)
        {
            this.dbPath = dbPath;
            _database = new SQLiteAsyncConnection(dbPath);
            CreateTables();
        }

        public void TestLoad()
        {
            Load ld = new Load();
            ld.LoadID = Guid.NewGuid();
            SQLiteConnection con = new SQLiteConnection(dbPath);
            con.Insert(ld);

        }
        public void CreateDisplayView()
        {
            SQLiteConnection con = new SQLiteConnection(dbPath);

            con.Execute(@"CREATE VIEW LoadDisplay
as
select 
    Load.LoadID,
    Load.Name as LoadName,
	Load.LoadState as LoadState,
    Rifle.Name as RifleName,
    CaliberMaster.Name as CaliberName,
    PowderManufacturerMaster.Name as PowderManfName,
    PowderModelMaster.Name as PowderName,
    BulletManufacturerMaster.Name as BulletManfName,
    BulletModelMaster.Name as BulletName,
    PrimerManufacturerMaster.Name as PrimerManfName,
    PrimerModelMaster.Name as PrimerName,
    BrassMaster.Name as BrassName,
    Load.StartingPowderCharge,
	Load.StartingCOAL,
    Load.COAL,
    Load.ShotsPerVariation,
    Load.VaryByCOAL,
    Load.VaryByPowderCharge,
    Load.VaryManually as VaryByManual,
    Load.PowderVariations,
    Load.ManualVariations,
    Load.PowderVariationAmount,
    Load.PowderCharge,
    Load.CoalVariations,
    Load.COALVariationAmount,
    BulletModelMaster.Weight as BulletWeight
from Load
     LEFT OUTER JOIN Rifle on Load.RifleID = Rifle.ID
     LEFT OUTER JOIN CaliberMaster on Rifle.CaliberID = CaliberMaster.CaliberID
     LEFT OUTER JOIN PowderManufacturerMaster on Load.PowderManfID = PowderManufacturerMaster.PowderManufacturerMasterID
     LEFT OUTER JOIN PowderModelMaster on Load.PowderModelID = PowderModelMaster.PowderModelMasterID
     LEFT OUTER JOIN BulletManufacturerMaster on Load.BulletManfID = BulletManufacturerMaster.BulletManufacturerMasterID
     LEFT OUTER JOIN BulletModelMaster on Load.BulletWeightID = BulletModelMaster.BulletModelID
     LEFT OUTER JOIN PrimerManufacturerMaster on Load.PrimerManfID = PrimerManufacturerMaster.PrimerManufacturerId
     LEFT OUTER JOIN PrimerModelMaster on Load.PrimerModelID = PrimerModelMaster.PrimerModelId
     LEFT OUTER JOIN BrassMaster on Load.CaseManfID = BrassMaster.BrassMasterId");

        }

        public void InsertTestLoadData()
        {
            Rifle rifle = new Rifle
            {
                Name = "F-Class",
                ID = Guid.NewGuid(),
                CaliberID = Guid.Parse("d8dd263f-6103-4c02-bbfc-dc6076fa867e")
            };

            _database.InsertAsync(rifle).Wait();

            Load ld = new Load();
            ld.LoadID = Guid.NewGuid();
            ld.Name = "Test Load";
            ld.RifleID = rifle.ID;
            ld.PowderManfID = Guid.Parse("5ae889a2-180b-4a91-ba56-a503a7ed3b4a");
            ld.PowderModelID = Guid.Parse("1b29a807-3463-482d-80a8-1350600559bf");
            ld.BulletManfID = Guid.Parse("6dc99f44-404f-4240-bdd9-800a923b4c1e");
            ld.BulletModelID = Guid.Parse("57ed6239-1048-47b1-a1a1-d63282e54ae3");
            ld.PrimerModelID = Guid.Parse("9a43a8ca-1a7d-4e05-bcb3-b3dee0fb11df");
            ld.PrimerManfID = Guid.Parse("3bc4fffe-4955-4ca4-bde1-75d08b6f3f6e");
            ld.CaseManfID = Guid.Parse("2692aea2-00ca-47f6-bc32-566f68f371e3");
            ld.StartingPowderCharge = 40.0F;
            ld.COAL = 2.800F;

            _database.InsertAsync(ld).Wait();
        }

        public void LoadCaliberMaster(StreamReader caliberStream) 
        {
            //CaliberID,Name,UserDefined,Diameter
            foreach (var line in Csv.CsvReader.ReadFromStream(caliberStream.BaseStream))
            {
                var cm = new CaliberMaster();
                cm.CaliberID = Guid.NewGuid();
                cm.Name = line["Name"].Trim();
                cm.BulletDiameter = decimal.Parse(line["Diameter"].Trim());
                _database.InsertAsync(cm).Wait();
            }
            caliberStream.Close();
        }

        public void LoadBulletMaster(string rawBulletDb, string masterDb)
        {
            var sourceDb = new SQLiteConnection(rawBulletDb);
            var targetDb = new SQLiteConnection(masterDb);

            var manfQuery = sourceDb.Query<BulletMasterProfile>("SELECT DISTINCT Manufacturer FROM BulletMasterProfile");
            foreach(var bulletProfile in manfQuery)
            {
                var currentManfID = Guid.NewGuid();
                targetDb.Insert(new BulletManufacturerMaster(bulletProfile.Manufacturer, currentManfID));
                var modelQuery = sourceDb.Table<BulletMasterProfile>().Where(v => v.Manufacturer.Equals(bulletProfile.Manufacturer));
                foreach(var model in modelQuery)
                {
                    targetDb.Insert(new BulletModelMaster(Guid.NewGuid(), currentManfID, model.Model, model.Caliber, model.Weight));
                }
            }
        }

        public void LoadPowderManufacturerMaster(StreamReader powderStream)
        {
            foreach (var line in Csv.CsvReader.ReadFromStream(powderStream.BaseStream))
            {
                var manf = line[0].Trim();
                _database.InsertAsync(new PowderManufacturerMaster(manf)).Wait();
            }
            powderStream.Close();
        }

        public void LoadPowderMaster(StreamReader powderStream)
        {
            // could be more efficient if we can assume the master data file is sorted, but, BAH
            foreach(var line in Csv.CsvReader.ReadFromStream(powderStream.BaseStream))
            {
                var manf = line[0].Trim();
                var model = line[1].Trim();

                
                var query = _database.Table<PowderManufacturerMaster>().Where(v => v.Name.Equals(manf));
                // this will go poof if we did not load the manf master first (but, what the hell)
                var task = query.FirstAsync();
                task.Wait();

                var pmm = task.Result;
                
                _database.InsertAsync(new PowderModelMaster(model, pmm.PowderManufacturerMasterID)).Wait();
            }
        }

        public void LoadPrimerManufacturerMaster(StreamReader primerStream)
        { 
            foreach(var line in Csv.CsvReader.ReadFromStream(primerStream.BaseStream))
            {
                var primerManf = new PrimerManufacturerMaster();
                primerManf.PrimerManufacturerId = Guid.NewGuid();
                primerManf.Name = line[0].Trim();
                _database.InsertAsync(primerManf).Wait();
            }
        }

        public void LoadBrassMaster(StreamReader brassStream)
        {
            foreach (var line in Csv.CsvReader.ReadFromStream(brassStream.BaseStream))
            {
                var brassManf = new BrassMaster();
                brassManf.BrassMasterId= Guid.NewGuid();
                brassManf.Name = line[0].Trim();
                _database.InsertAsync(brassManf).Wait();
            }
        }

        public void LoadPrimerModelMaster(StreamReader primerStream)
        {
            foreach (var line in Csv.CsvReader.ReadFromStream(primerStream.BaseStream))
            {
                var primerModel = new PrimerModelMaster();
                primerModel.PrimerModelId= Guid.NewGuid();

                var manf = line[0].Trim();
                var query = _database.Table<PrimerManufacturerMaster>().Where(c => c.Name.Equals(manf));
                var task = query.FirstAsync();
                task.Wait();

                primerModel.PrimerManfId = task.Result.PrimerManufacturerId;
                primerModel.Name = line[1].Trim();

                _database.InsertAsync(primerModel).Wait();
            }
        }


        private void CreateTables()
        {
            _database.CreateTableAsync<Rifle>().Wait();
            _database.CreateTableAsync<Bullet>().Wait();
            _database.CreateTableAsync<BulletModelMaster>().Wait();
            _database.CreateTableAsync<BulletManufacturerMaster>().Wait();
            _database.CreateTableAsync<Caliber>().Wait();
            _database.CreateTableAsync<CaliberMaster>().Wait();
            _database.CreateTableAsync<Load>().Wait();
            _database.CreateTableAsync<Powder>().Wait();
            _database.CreateTableAsync<PowderManufacturerMaster>().Wait();
            _database.CreateTableAsync<PowderModelMaster>().Wait();
            _database.CreateTableAsync<LoadStringShot>().Wait();
            _database.CreateTableAsync<PrimerManufacturerMaster>().Wait();
            _database.CreateTableAsync<PrimerModelMaster>().Wait();
            _database.CreateTableAsync<BrassMaster>().Wait();
            _database.CreateTableAsync<LoadString>().Wait();
            _database.CreateTableAsync<ManualVariation>().Wait();
        }
    }
}
