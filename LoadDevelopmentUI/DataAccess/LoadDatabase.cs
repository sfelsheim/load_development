using System.Collections.Generic;
using SQLite;
using DataAccess.Model;
using System;
using System.Linq;

namespace DataAccess
{
    public class BulletComparer : IEqualityComparer<BulletModelMaster>
    {
        public bool Equals(BulletModelMaster x, BulletModelMaster y)
        {
            if (Object.ReferenceEquals(x, y)) 
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Name == y.Name;
        }

        public int GetHashCode(BulletModelMaster obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.Name.GetHashCode();
        }
    }

    public class LoadDatabase
    {
        readonly object locker = new object();
        readonly SQLiteConnection database;

        public LoadDatabase(string databasePath)
        {
            lock (locker)
            {
                database = new SQLiteConnection(databasePath);
            }
        }

        public List<Rifle> GetRifles()
        {
            lock (locker)
            {
                return database.Table<Rifle>().ToList();
            }
        }

        public void UpdateLoadStringShot(LoadStringShot shot)
        {
            lock(locker)
            {
                database.Update(shot);
			}
        }

        public LoadString GetLoadString(Guid loadStringId)
        { 
			lock (locker)
            {
                return database.Table<LoadString>().Where(c => c.LoadStringID == loadStringId).Take(1).ToList()[0];
			}
		}

        public List<LoadStringShot> GetLoadStringShots(Guid loadID, Guid loadStringID)
        {
            lock (locker)
            { 
                var ret = database.Table<LoadStringShot>().Where(c => c.LoadId == loadID && c.LoadStringId == loadStringID).ToList();

                foreach(var shot in ret)
                {
                    shot.TheLoadString = GetLoadString(shot.LoadStringId);
				}

                return ret;
			}
        }

        public List<CaliberMaster> GetCalibers()
        {
            lock (locker)
            {
                return database.Table<CaliberMaster>().OrderBy(c => c.BulletDiameter).ToList();
            }
        }

        public BulletManufacturerMaster GetBulletManf(Guid id)
        { 
            lock (locker)
            {
                return database.Table<BulletManufacturerMaster>().Where(c => c.BulletManufacturerMasterID == id).First();
			}
		}
        public List<BulletManufacturerMaster> GetBulletManf()
        {
            lock (locker)
            {
                return database.Table<BulletManufacturerMaster>().OrderBy(c => c.Name).ToList();
            }
        }

        public Load GetLoad(Guid loadId)
        {
            lock (locker)
            {
                return database.Table<Load>().Where(c => c.LoadID == loadId).ElementAt(0);
            }
        }

        public PowderModelMaster GetPowderModel(Guid id)
        { 
            lock(locker)
            {
                return database.Table<PowderModelMaster>().Where(c => c.PowderModelMasterID == id).First();
			}
		}
        public List<PowderModelMaster> GetPowderModels(Guid manf)
        {
            lock (locker)
            {
                return database.Table<PowderModelMaster>().Where(a => a.PowderManufacturerMasterID == manf)
                    .OrderBy(c => c.Name).ToList();
            }
        }

        public List<PrimerManufacturerMaster> GetPrimerManufacturer()
        {
            lock (locker)
            {
                return database.Table<PrimerManufacturerMaster>().OrderBy(c => c.Name).ToList();
            }
        }

        public List<PrimerModelMaster> GetPrimerModels(Guid manfId)
        {
            lock (locker)
            {
                return database.Table<PrimerModelMaster>().Where(
                    c => c.PrimerManfId == manfId).OrderBy(v => v.Name).ToList();
            }
        }

        public List<LoadString> GetLoadStrings(Guid loadId)
        { 
            lock (locker)
            {
                return database.Table<LoadString>().Where(c => c.LoadID == loadId).ToList();
			}
		}

        public void DeleteLoadStrings(Guid loadId)
        {
            lock (locker)
            {
                database.Execute("DELETE FROM LoadString WHERE LoadID = '?'", loadId.ToString());
            }
		}

        public void SaveLoadStringShot(LoadStringShot shot)
        {
            lock(locker)
            {
                database.Insert(shot);
			}
        }

        public void SaveLoadString(LoadString loadString)
        { 
            lock (locker)
            {
                database.Update(loadString);
			}
		}

        public void SaveLoadString(List<LoadString> loadString)
        {
            lock (locker)
            {
                database.InsertAll(loadString);
			}
        }

        public List<BrassMaster> GetBrassMasters()
        {
            lock (locker)
            {
                return database.Table<BrassMaster>().OrderBy(c => c.Name).ToList();
            }
        }

        public PowderManufacturerMaster GetPowderManf(Guid id)
        { 
            lock(locker)
            {
                return database.Table<PowderManufacturerMaster>().Where(c => c.PowderManufacturerMasterID == id).First();
			}
		}

        public List<PowderManufacturerMaster> GetPowderManf()
        {
            lock (locker)
            {
                return database.Table<PowderManufacturerMaster>().OrderBy(c => c.Name).ToList();
            }
        }

        public CaliberMaster GetCaliber(Guid id)
        {
            lock (locker)
            {
                return database.Table<CaliberMaster>().Where(c => c.CaliberID == id).First();
            }
        }

        public List<BulletModelMaster> GetBullets(Guid manfId, decimal caliber)
        {
            lock (locker)
            {
                var bulletMasterDupes = database.Table<BulletModelMaster>().Where(
                    c => c.BulletManufacturerID == manfId && c.Diameter == caliber).ToList();

                return bulletMasterDupes.Distinct(new BulletComparer()).ToList();
            }
        }

        public void DeleteManualVariation(Guid loadStringID)
        {
            lock (locker)
            {
                database.Delete(new ManualVariation { ManualVariationID = loadStringID });
	        }
        }

        public void InsertManualVariation(ManualVariation manualVariation)
        {
            lock (locker)
            {
                database.Insert(manualVariation);
	        }
        }

        public List<ManualVariation> GetManualVariations(Guid loadId)
        { 
            lock(locker)
            {
                return database.Table<ManualVariation>().Where(c => c.LoadID == loadId).ToList();
	        }
	    }
        public void SaveRifle(Rifle rifle)
        {
            lock (locker)
            {
                database.Insert(rifle);
            }
        }

        public void SaveCaliber(CaliberMaster caliber)
        {
            lock (locker)
            {
                database.Insert(caliber);
            }
        }

        public void NewLoad(Load load)
        {
            lock (locker)
            {
                database.Insert(load);
            }
        }

        public void UpdateLoad(Load load)
        {
            lock (locker)
            {
                database.Update(load);
            }
        }

        public List<BulletModelMaster> GetBulletWeights(string name, Guid manfId, decimal diameter)
        {
            lock (locker)
            {
                return database.Table<BulletModelMaster>().Where(c => c.BulletManufacturerID == manfId 
                        && c.Name == name && c.Diameter == diameter).ToList();
            }
        }

        public Load CreateNewLoad()
        {
            lock (locker)
            {
                var load = new Load { LoadID = Guid.NewGuid() };
                database.Insert(load);

                return load;
            }
        }

        public List<Load> GetLoads()
        {
            lock (locker)
            {
                return database.Table<Load>().ToList();
            }
        }

        public List<LoadDisplay> GetDisplayLoads(string state)
        {
            lock (locker)
            {
               return database.Table<LoadDisplay>().Where(c => c.LoadState == state).ToList();
            }
        }

        public LoadDisplay GetDisplayLoad(Guid loadId)
        { 
            lock (locker)
            {
                return database.Table<LoadDisplay>().Where(c => c.LoadID == loadId).First();
			}
		}
        public Rifle GetRifle(Guid rifleID)
        {
            lock (locker)
            {
                return database.Table<Rifle>().Where(c => c.ID == rifleID).First();
            }
        }

        public BulletModelMaster GetBullet(Guid bulletModelID)
        {
            lock (locker)
            {
                return database.Table<BulletModelMaster>().Where(c => c.BulletModelID == bulletModelID).First();
		    }
        }

        public PrimerManufacturerMaster GetPrimerManufacturer(Guid primerManfID)
        {
            lock (locker)
            {
                return database.Table<PrimerManufacturerMaster>().Where(c => c.PrimerManufacturerId == primerManfID).First();
			}
        }

        public PrimerModelMaster GetPrimerModel(Guid primerModelID)
        {
            lock (locker)
            {
                return database.Table<PrimerModelMaster>().Where(c => c.PrimerModelId == primerModelID).First();
            }
        }

        public BrassMaster GetBrassMaster(Guid caseManfID)
        {
            lock (locker)
            {
                return database.Table<BrassMaster>().Where(c => c.BrassMasterId == caseManfID).First();
			}
        }

        public void DeleteLoad(Guid loadID)
        {
            lock (locker)
            {
                database.Delete(new Load { LoadID = loadID });
			}
        }
    }
}
