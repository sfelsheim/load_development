using System;
using SQLite;
namespace DataAccess.Model
{
    public class LoadString
    {
        [PrimaryKey]
        public Guid LoadStringID { get; set; }

        public Guid LoadID { get; set; }

        public string ID { get; set; }

        public int NumRounds { get; set; }

        public float PowderCharge { get; set; }

        public float Coal { get; set; }

        public int AvgVelocity { get; set; }

        public int EsVelocity { get; set; }

        public float SdVelocity { get; set; }

        [Ignore]
        public bool ShowSelect { get; } = false;
        [Ignore]
        public bool VaryByPowderCharge { get; set; } = true;

        [Ignore]
        public string Display
        { 
            get
            {
                return string.Format("Test Shot String {0} ",
                    ID, NumRounds, PowderCharge, Coal);
			}
		}

        [Ignore]
        public string Detail
        { 
            get
            {
                string value;
                if (VaryByPowderCharge)
                {
                    value = string.Format("{0} at {1:F1} gr ",
                    NumRounds, PowderCharge);
                }
                else
                {
                    value = string.Format("{0} at {1:F3} COAL ",
                        NumRounds, Coal);
                }

                if (AvgVelocity > 0)
                {
                    value += string.Format(" :  AVG VEL {0}, SD {1:F1}, ES {2}",
                        AvgVelocity, SdVelocity, EsVelocity);
				}

                return value;
			}
		}
    }
}
