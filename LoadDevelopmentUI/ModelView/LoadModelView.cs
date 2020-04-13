using System;
using System.Collections.Generic;
using System.ComponentModel;
using DataAccess;
using DataAccess.Model;

namespace LoadDevelopmentUI.ModelView
{
    public class LoadModelView : INotifyPropertyChanged
    {
        // persistance of user values
        private LoadDatabase database;

        // dah current load being worked on, like the name says
        private Load currentLoad;

        // value bindings for controls
        private List<Rifle> rifles = new List<Rifle>();
        private List<CaliberMaster> calibers = new List<CaliberMaster>();
        private List<PowderManufacturerMaster> powderManf = new List<PowderManufacturerMaster>();
        private List<PowderModelMaster> powderModel = new List<PowderModelMaster>();
        private List<BulletManufacturerMaster> bulletManf = new List<BulletManufacturerMaster>();
        private List<BulletModelMaster> bulletModels = new List<BulletModelMaster>();
        private List<BulletModelMaster> bulletWeights = new List<BulletModelMaster>();
        private List<PrimerManufacturerMaster> primerManf = new List<PrimerManufacturerMaster>();
        private List<PrimerModelMaster> primerModel = new List<PrimerModelMaster>();

        // holders for user selected values as they change them
        private string loadName = "";
        private Rifle selectedRifle;
        private BulletManufacturerMaster selectedBulletManf;
        private PowderManufacturerMaster selectedPowderManf;
        private PowderModelMaster selectedPowderModel;
        private BulletModelMaster selectedBulletModel;
        private BulletModelMaster selectedBulletWeight;
        private PrimerManufacturerMaster selectedPrimerManf;
        private PrimerModelMaster selectedPrimerModel;
        private BrassMaster selectedBrassMaster;
        private string selectedCaseOAL;
        private string selectedHeadspace;
        private bool isVaryByPowderCharge = true;
        private bool isVaryByCoal = false;
        private bool isManualVary = false;
        private string powderVariations;
        private string shotsPerVariation;
        private string coalVariations;
        private string powderVariationAmount;
        private string coalVariationAmount;
        private string startingPowderCharge;
        private string startingCoal;
        private string coal;
        private string powderCharge;
        private string loadDescription; 
        private List<LoadString> loadStrings = new List<LoadString>();



        public LoadModelView(LoadDatabase database, Load load)
        {
            this.database = database;
            this.currentLoad = load;

            rifles = database.GetRifles();
            calibers = database.GetCalibers();
            powderManf = database.GetPowderManf();
            bulletManf = database.GetBulletManf();
            primerManf = database.GetPrimerManufacturer();

            if (!string.IsNullOrEmpty(currentLoad.Name))
                LoadName = currentLoad.Name;

            if (currentLoad.RifleID != Guid.Empty)
                SelectedRifle = GetRifle(currentLoad.RifleID);

            if (currentLoad.PowderManfID != Guid.Empty)
                SelectedPowderManf = GetPowderManf(currentLoad.PowderManfID);

            if (currentLoad.PowderModelID != Guid.Empty)
                SelectedPowderModel = GetPowderModel(currentLoad.PowderModelID);

            if (currentLoad.BulletManfID != Guid.Empty)
                SelectedBulletManf = GetBulletManf(currentLoad.BulletManfID);

            if (currentLoad.BulletModelID != Guid.Empty)
            {
                var bullet = GetBullet(currentLoad.BulletModelID);

                SelectedBullet = bullet;

                BulletWeights = GetBulletWeights(
                    bullet.Name,
                    bullet.BulletManufacturerID,
                    bullet.Diameter);
            }

            if (currentLoad.BulletWeightID != Guid.Empty)
                SelectedBulletWeight = GetBullet(currentLoad.BulletWeightID);

            if (currentLoad.PrimerManfID != Guid.Empty)
            {
                SelectedPrimerManf = GetPrimerManufacturer(currentLoad.PrimerManfID);
            }

            if (currentLoad.PrimerModelID != Guid.Empty)
                SelectedPrimerModel = GetPrimerModel(currentLoad.PrimerModelID);

            if (currentLoad.CaseManfID != Guid.Empty)
                SelectedBrass = GetBrassMaster(currentLoad.CaseManfID);

            SelectedIsVaryByPowderCharge = currentLoad.VaryByPowderCharge;
            SelectedIsVaryByCoal = currentLoad.VaryByCOAL;
            SelectedManualVary = currentLoad.VaryManually;

            if (currentLoad.ShotsPerVariation > 0)
                SelectedShotsPerVariation = currentLoad.ShotsPerVariation.ToString();

            if (currentLoad.VaryByPowderCharge)
            {
                if (currentLoad.PowderVariations > 0)
                    SelectedVariations = currentLoad.PowderVariations.ToString();

                if (currentLoad.PowderVariationAmount > 0)
                    SelectedVariationAmount = currentLoad.PowderVariationAmount.ToString("F1");

                if (currentLoad.StartingPowderCharge > 0)
                    SelectedStarting = currentLoad.StartingPowderCharge.ToString("F1");

                if (currentLoad.COAL > 0)
                    SelectedCoal = currentLoad.COAL.ToString("F3");
            }
            else
            {
                if (currentLoad.CoalVariations > 0)
                    SelectedVariations = currentLoad.CoalVariations.ToString();

                if (currentLoad.COALVariationAmount > 0)
                    SelectedVariationAmount = currentLoad.COALVariationAmount.ToString("F3");

                if (currentLoad.StartingCOAL > 0)
                    SelectedStarting = currentLoad.StartingCOAL.ToString("F3");

                if (currentLoad.PowderCharge > 0)
                    SelectedPowderCharge = currentLoad.StartingPowderCharge.ToString("F1");
            }
        }

        public void DeleteManualVariation(Guid loadStringID)
        {
            database.DeleteManualVariation(loadStringID);
            currentLoad.ManualVariations = currentLoad.ManualVariations - 1;
            database.UpdateLoad(currentLoad);
        }

        public List<ManualVariation> GetManualVariations(Guid loadID)
        {
            return database.GetManualVariations(loadID);
        }

        public void AddManualVariation(Guid loadId, int numRounds, float coal, float powderCharge)
        {
            database.InsertManualVariation(new ManualVariation
            {
                LoadID = loadId,
                ManualVariationID = Guid.NewGuid(),
                Coal = coal,
                NumRounds = numRounds,
                PowderCharge = powderCharge
            });
            currentLoad.ManualVariations = currentLoad.ManualVariations + 1;
            database.UpdateLoad(currentLoad);
        }

        public void SaveLoadRecipe(List<LoadString> loadRecipe)
        {
			database.SaveLoadString(loadRecipe);
            foreach(var loadString in loadRecipe)
            { 
                for(int i = 0; i < loadString.NumRounds; ++i)
                {
                    LoadStringShot shot = new LoadStringShot();
                    shot.LoadStringShotId = Guid.NewGuid();
                    shot.LoadStringId = loadString.LoadStringID;
                    shot.LoadId = loadString.LoadID;
                    database.SaveLoadStringShot(shot);
				}
			}
        }

        public string LoadDescription
        { 
            get
            {
                return loadDescription;
			}
            set
            { 
                if (value != loadDescription)
                {
                    loadDescription = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LoadDescription"));
				}
			}
		}

        public List<LoadString> LoadStrings
        { 
            get { return loadStrings;  }
            set
            { 
                if (!loadStrings.Equals(value))
                {
                    loadStrings = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LoadStrings"));
				}
			}
		}

        #region User Selection Persisance Properties

        public string SelectedPowderCharge
        {
            get { return powderCharge;  }
            set
            {
                if (powderCharge != value)
                {
                    powderCharge = value;
                    float defVal;
					bool ok = float.TryParse(powderCharge, out defVal);
                    if (ok)
                    {
                        currentLoad.PowderCharge = defVal;
                        database.UpdateLoad(currentLoad);
                    }
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedPowderCharge"));
                }
            }
        }

        public void SetCurrentLoadToTest()
        {
            currentLoad.LoadState = Load.TEST_STATE;
            currentLoad.IsNew = true;
            database.UpdateLoad(currentLoad);
        }

        public bool SelectedManualVary
        { 
            get { return isManualVary;  }
            set
            { 
                if (isManualVary !=  value)
                {
                    isManualVary = value;
                    currentLoad.VaryManually = isManualVary;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedManualVary"));
                }
	        }
	    }
        public bool SelectedIsVaryByCoal
        {
            get { return isVaryByCoal;  }
            set
            {
                if (isVaryByCoal != value)
                {
                    isVaryByCoal = value;
                    currentLoad.VaryByCOAL = isVaryByCoal;
                    database.UpdateLoad(currentLoad);
                    SelectedStarting = null;
                    SelectedCoal = null;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedIsVaryByCoal"));
                }
            }
        }

        public string SelectedCoal
        {
            get { return coal; }
            set
            {
                if (coal != value)
                {
                    coal = value;
                    float val;
                    bool ok = float.TryParse(coal, out val);
                    if (ok)
                    {
                        currentLoad.COAL = val;
                        database.UpdateLoad(currentLoad);
                    }
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedCoal"));
                }
            }
        }

        public string SelectedStarting
        {
            get
            {
                if (SelectedIsVaryByPowderCharge)
                    return startingPowderCharge;
                else
                    return startingCoal;
			}
            set
            {
                if (SelectedIsVaryByPowderCharge)
                { 
                    if (startingPowderCharge != value)
                    {
                        startingPowderCharge = value;
                        float val;
                        bool ok = float.TryParse(startingPowderCharge, out val);
                        if (ok)
                        {
                            currentLoad.StartingPowderCharge = val;
                            database.UpdateLoad(currentLoad);
                        }
						PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedStarting"));
					}
				}
                else
                {
                    if (startingCoal != value)
                    {
                        startingCoal = value;
                        float val;
                        bool ok = float.TryParse(startingCoal, out val);
                        if (ok)
                        {
                            currentLoad.StartingCOAL = val;
                            database.UpdateLoad(currentLoad);
                        }
						PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedStarting"));
                    }
                }
            }
        }

        public string SelectedVariationAmount 
        {
            get 
			{
                if (isVaryByPowderCharge)
                    return powderVariationAmount;
                else
                    return coalVariationAmount;
			}

            set
            {
                if (isVaryByPowderCharge)
                {
                    if (powderVariationAmount != value)
                    {
                        powderVariationAmount = value;
                        float val;
                        bool ok = float.TryParse(powderVariationAmount, out val);
                        if (ok)
                        {
                            currentLoad.PowderVariationAmount = val;
							database.UpdateLoad(currentLoad);
						}
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedVariationAmount"));
                    }
                }
                else
                {
                    if (coalVariationAmount != value)
                    {
                        coalVariationAmount = value;
                        float val;
                        bool ok = float.TryParse(coalVariationAmount, out val);
                        if (ok)
                        {
                            currentLoad.COALVariationAmount = val;
                            database.UpdateLoad(currentLoad);
                        }
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedVariationAmount"));
                    }
                }
            }
        }

        public string SelectedShotsPerVariation
        {
            get { return shotsPerVariation; }
            set
            {
                if (shotsPerVariation != value)
                {
                    shotsPerVariation = value;
                    int val;
                    bool ok = int.TryParse(shotsPerVariation, out val);
                    if (ok)
                    {
                        currentLoad.ShotsPerVariation = val;
						database.UpdateLoad(currentLoad);
					}
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedShotsPerVariation"));
                }
            }
        }

        public string SelectedVariations
        {
            get
            {
                if (isVaryByPowderCharge)
                    return powderVariations;
                else
                    return coalVariations;
            }

            set
            {
                if (isVaryByPowderCharge)
                {
                    if (powderVariations != value)
                    {
                        powderVariations = value;
                        int val;
                        bool ok = int.TryParse(powderVariations, out val);
                        if (ok)
                        {
                            currentLoad.PowderVariations = val;
							database.UpdateLoad(currentLoad);
						}
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedVariations"));
                    }
                }
                else
                {
                    if (coalVariations != value)
                    {
                        coalVariations = value;
                        int val;
                        bool ok = int.TryParse(coalVariations, out val);
                        if (ok)
                        {
                            currentLoad.CoalVariations = val;
                            database.UpdateLoad(currentLoad);
                        }
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedVariations"));
                    }
                }
            }
        }

        public bool SelectedIsVaryByPowderCharge
        {
            get { return isVaryByPowderCharge; }
            set
            {
                if (isVaryByPowderCharge != value)
                {
                    isVaryByPowderCharge = value;
                    currentLoad.VaryByPowderCharge = isVaryByPowderCharge;
                    database.UpdateLoad(currentLoad);
					SelectedStarting = null;
					SelectedPowderCharge = null;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedIsVaryByPowderCharge"));
                }
            }
        }

        public string SelectedHeadspace
        {
            get { return selectedHeadspace; }
            set
            {
                if (selectedHeadspace != value)
                {
                    selectedHeadspace = value;
                    float val;
                    bool ok = float.TryParse(selectedHeadspace, out val);
                    if (ok)
                    {
                        currentLoad.CaseHeadspace = val;
						database.UpdateLoad(currentLoad);
					}
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedHeadspace"));
                }
            }
        }

        public string SelectedCaseOAL
        {
            get { return selectedCaseOAL;  }
            set
            {
                if (selectedCaseOAL != value)
                {
                    selectedCaseOAL = value;
                    float val;
                    bool ok = float.TryParse(selectedCaseOAL, out val);
                    if (ok)
                    {
                        currentLoad.CaseOAL = val;
                        database.UpdateLoad(currentLoad);
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedCaseOAL"));
                }
            }
        }

        public BrassMaster SelectedBrass
        {
            get { return selectedBrassMaster;  }
            set
            {
                if (selectedBrassMaster != value)
                {
                    selectedBrassMaster = value;
                    currentLoad.CaseManfID = selectedBrassMaster.BrassMasterId;
                    database.UpdateLoad(currentLoad);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedBrass"));
                }
            }
        }

        public PowderModelMaster GetPowderModel(Guid id)
        {
            return database.GetPowderModel(id);
        }

        public BulletManufacturerMaster GetBulletManf(Guid bulletManfID)
        {
            return database.GetBulletManf(bulletManfID);
        }

        public BulletModelMaster GetBullet(Guid bulletModelID)
        {
            return database.GetBullet(bulletModelID);
        }

        public PrimerManufacturerMaster GetPrimerManufacturer(Guid primerManfID)
        {
            return database.GetPrimerManufacturer(primerManfID);
        }

        public PowderManufacturerMaster GetPowderManf(Guid id)
        {
            return database.GetPowderManf(id);
        }

        public PrimerModelMaster GetPrimerModel(Guid id)
        {
            return database.GetPrimerModel(id);
        }

        public BrassMaster GetBrassMaster(Guid id)
        {
            return database.GetBrassMaster(id);
        }

        public Rifle GetRifle(Guid id)
        {
            return database.GetRifle(id);
        }

        public PrimerModelMaster SelectedPrimerModel
        {
            get { return selectedPrimerModel;  }
            set
            {
                if (selectedPrimerModel != value)
                {
                    if (value != null)
                    {
                        selectedPrimerModel = value;
                        currentLoad.PrimerModelID = selectedPrimerModel.PrimerModelId;
                        database.UpdateLoad(currentLoad);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedPrimerModel"));
                    }
                }
            }
        }

        public PrimerManufacturerMaster SelectedPrimerManf
        {
            get { return selectedPrimerManf;  }
            set
            {
                if (selectedPrimerManf != value)
                {
                    selectedPrimerManf = value;
                    if (value != null)
                    {
                        currentLoad.PrimerManfID = selectedPrimerManf.PrimerManufacturerId;
                        database.UpdateLoad(currentLoad);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedPrimerManf"));
                    }
                }
            }
        }

        public PowderManufacturerMaster SelectedPowderManf 
        {
            get { return selectedPowderManf; }
            set
            {
                if (selectedPowderManf != value)
                {
                    if (value != null)
                    {
                        selectedPowderManf = value;
                        currentLoad.PowderManfID = selectedPowderManf.PowderManufacturerMasterID;
                        database.UpdateLoad(currentLoad);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedPowderManf"));
                    }
                }
            }
        }

        public PowderModelMaster SelectedPowderModel
        {
            get { return selectedPowderModel; }
            set
            {
                if (selectedPowderModel != value)
                {
                    selectedPowderModel = value;
                    if (value != null)
                    {
                        currentLoad.PowderModelID = selectedPowderModel.PowderModelMasterID;
                        database.UpdateLoad(currentLoad);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedPowderModel"));
                    }
                }
            }
        }

        public Rifle SelectedRifle 
        {
            get { return selectedRifle; }
            set
            {
                if (selectedRifle != value)
                {
                    selectedRifle = value;
                    if (value != null)
                    {
                        currentLoad.RifleID = selectedRifle.ID;
                        database.UpdateLoad(currentLoad);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedRifle"));
                    }
                }
            } 
        }

        public CaliberMaster SelectedCaliber { get; set; }

        public BulletManufacturerMaster SelectedBulletManf 
        { 
            get { return selectedBulletManf;  }

            set
            {
                if (selectedBulletManf != value)
                {
                    selectedBulletManf = value;
                    if (value != null)
                    {
                        currentLoad.BulletManfID = selectedBulletManf.BulletManufacturerMasterID;
                        database.UpdateLoad(currentLoad);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedBulletManf"));
                    }
                }
            }
        }

        public BulletModelMaster SelectedBulletWeight
        { 
            get { return selectedBulletWeight; }
            set
            { 
                if (selectedBulletWeight != value)
                {
                    selectedBulletWeight = value;
                    if (value != null)
                    {
                        currentLoad.BulletWeightID = selectedBulletWeight.BulletModelID;
                        database.UpdateLoad(currentLoad);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedBulletWeight"));
                    }
				}
			}
		}

        public BulletModelMaster SelectedBullet
        {
            get { return selectedBulletModel;  }
            set
            {
                if (selectedBulletModel != value)
                {
                    selectedBulletModel = value;
                    if (value != null)
                    {
                        currentLoad.BulletModelID = selectedBulletModel.BulletModelID;
                        database.UpdateLoad(currentLoad);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedBullet"));
                    }
                }
            }
        }

        public string LoadName
        {
            get { return loadName; }
            set
            {
                if (loadName != value)
                {
                    if (value != null)
                    {
                        loadName = value;
                        currentLoad.Name = loadName;
                        database.UpdateLoad(currentLoad);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LoadName"));
                    }
                }
            }
        }
        #endregion

        public CaliberMaster GetCaliber(Guid id)
        {
            return database.GetCaliber(id);
        }


        #region Binding Values
        public List<PrimerManufacturerMaster> PrimerManufacturer
        {
            get 
            {
                return primerManf;
            }
            set
            {
                if (!primerManf.Equals(value))
                {
                    primerManf = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrimerManufacturer"));
                }
            }
        }

        public List<PrimerModelMaster> PrimerModel
        {
            get
            {
                return primerModel;
            }
            set
            {
                if (!primerModel.Equals(value))
                {
                    primerModel = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrimerModel"));
                }
            }
        }

        public List<Rifle> Rifles
        {
            get { return rifles; }

            set
            {
                if (!rifles.Equals(value))
                {
                    rifles = database.GetRifles();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rifles"));
                }
            }
        }

        public List<CaliberMaster> Calibers
        {
            get { return calibers; }
            set
            {
                if (!calibers.Equals(value))
                {
                    calibers = database.GetCalibers();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Calibers")); 
                }
            }
        }

        public List<BulletManufacturerMaster> BulletManf
        {
            get { return bulletManf; }
        }

        public List<BulletModelMaster> BulletModel
        {
            get { return bulletModels;  }
            set
            {
                if (!bulletModels.Equals(value))
                {
                    bulletModels = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BulletModel"));
                }
            }
        }

        public List<PowderManufacturerMaster> PowderManf
        {
            get { return powderManf;  }
            set
            { 
                if (!powderManf.Equals(value))
                {
                    powderManf = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PowderManf"));
				}
			}
        }

        public List<PowderModelMaster> PowderModel
        {
            get { return powderModel; }

            set
            {
                if (!powderModel.Equals(value))
                {
                    powderModel = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PowderModel"));
                }
            }
        }

        public List<BrassMaster> CaseManufacturer
        {
            get { return database.GetBrassMasters(); }
        }

        public List<BulletModelMaster> BulletWeights
        {
            get { return bulletWeights; }
            set
            {
                if (!bulletWeights.Equals(value))
                {
                    bulletWeights = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BulletWeights"));
                }
            }
        }
        #endregion

        public void SaveRifle(Rifle newRifle)
        {
            database.SaveRifle(newRifle);
            this.Rifles = database.GetRifles();
            SelectedRifle = newRifle;
        }

        public void SaveCaliber(CaliberMaster caliber)
        {
            database.SaveCaliber(caliber);
            this.Calibers = database.GetCalibers();
            SelectedCaliber = caliber;
        }

        public List<PrimerModelMaster> GetPrimerModels(Guid primerManufacturerId)
        {
            return database.GetPrimerModels(primerManufacturerId);
        }

        public List<PowderModelMaster> GetPowderModels(Guid manfId)
        {
            return database.GetPowderModels(manfId);
        }

        public List<BulletModelMaster> GetBulletModels(Guid manfId, decimal diameter)
        {
            return database.GetBullets( manfId, diameter);
        }

        public List<BulletModelMaster> GetBulletWeights(string name, Guid manfId, decimal diameter)
        {
            return database.GetBulletWeights(name, manfId, diameter);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
