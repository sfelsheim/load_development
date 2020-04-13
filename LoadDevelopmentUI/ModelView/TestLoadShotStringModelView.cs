using System;
using System.Collections.Generic;
using System.ComponentModel;
using DataAccess;
using DataAccess.Model;
using MathNet.Numerics.Statistics;

namespace LoadDevelopmentUI.ModelView
{
    public class TestLoadShotStringModelView : INotifyPropertyChanged
    {
        private List<LoadStringShot> loadStringShots = new List<LoadStringShot>();
        private LoadDatabase database;
        private Load currentLoad;
        private LoadString currentLoadString;
        private int avg;
        private int min;
        private int max;
        private int es;
        private float sd;


        public TestLoadShotStringModelView(LoadDatabase database, Load currentLoad, 
			LoadString currentLoadString)
        {
            this.currentLoad = currentLoad;
            this.currentLoadString = currentLoadString;
            this.database = database;

            loadStringShots = database.GetLoadStringShots(
					currentLoadString.LoadID, 
					currentLoadString.LoadStringID); 
			
        }

        public List<LoadStringShot> LoadStringShots
        { 
            get { return loadStringShots;  }
		}

        public void UpdateLoadStringShot(LoadStringShot shot)
        {
            database.UpdateLoadStringShot(shot);
        }

        public void UpdateStats()
        {
            Avg = ComputeAvg();
            Min = ComputeMin();
            Max = ComputeMax();
            Es = ComputeEs();
            Sd = ComputeSd();

            currentLoadString.AvgVelocity = this.avg;
            currentLoadString.EsVelocity = this.es;
            currentLoadString.SdVelocity = this.sd;

            database.SaveLoadString(currentLoadString);
        }

        public float PowderCharge
        { 
            get
            {
                return currentLoadString.PowderCharge;
			}
		}

        public float Coal
        { 
            get { return currentLoadString.Coal;  }
		}
        public int Avg
        {
            get
            {
                return avg;
            }

            set
            { 
                if (avg != value)
                {
                    avg = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Avg"));
                }
			}

        }

        public int Min
        { 
            get
            {
                return min;
			}
            set
            { 
                if (min != value)
                {
                    min = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Min"));
                }
			}
		}

        public int Max
        { 
            get
            {
                return max;	
            }
            set
            { 
                if (max != value)
                {
                    max = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Max"));
                }
			}
		}

        public float Sd
        {
            get 
			{
                return sd;
			}
            set
            { 
                if (sd != value)
                {
                    sd = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Sd"));
                }
			}
		}

        public int Es
        {
            get 
			{
                return es;
			}
            set
            { 
                if (es != value)
                {
                    es = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Es"));
                }
			}
		}

        public List<float> GetVelocities()
        {
            List<float> ret = new List<float>();
            foreach (var shot in loadStringShots)
            {
                if (shot.Velocity > 0 && shot.IsError == false)
                    ret.Add(shot.Velocity);
            }

            return ret;
		}

        public int ComputeAvg()
        {
            int sum = 0;
            int count = 0;
            foreach(var shot in loadStringShots)
            {
                if (shot.IsError)
                    continue;

                sum += shot.Velocity;
                if (shot.Velocity > 0)
                    ++count;
			}

            return sum == 0 ? 0 : sum / count;
		}

        public int ComputeMin()
        {
            var values = GetVelocities();
            if (values.Count == 0)
                return 0;
            return (int)ArrayStatistics.Minimum(values.ToArray());
		}

        public int ComputeMax()
        {
            var values = GetVelocities();
            if (values.Count == 0)
                return 0;

            return (int)ArrayStatistics.Maximum(values.ToArray());
		}

        public int ComputeEs()
        {
            return ComputeMax() - ComputeMin();
		}

        public float ComputeSd()
        {
            var x = ArrayStatistics.StandardDeviation(GetVelocities().ToArray());
            return double.IsNaN(x) ? 0 : (float)x;
		}

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
