﻿using System;
using SQLite;
namespace DataAccess.Model
{
    public enum VariationType
    { 
        ByPowder,
        ByCoal,
        Manual
    }

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
        public VariationType Variation { get; set; } = VariationType.ByPowder;

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
                if (Variation == VariationType.ByPowder)
                {
                    value = string.Format("{0} at {1:F1} gr ",
                    NumRounds, PowderCharge);
                }
                else if (Variation == VariationType.ByCoal)
                {
                    value = string.Format("{0} at {1:F3} COAL ",
                        NumRounds, Coal);
                }
                else
                {
                    value = string.Format("{0} at {1:F1} gr, {2:F3} COAL",
                        NumRounds, PowderCharge, Coal);
		        }

                if (AvgVelocity > 0)
                {
                    value += string.Format(" : AVG {0}, SD {1:F1}, ES {2}",
                        AvgVelocity, SdVelocity, EsVelocity);
				}

                return value;
			}
		}
    }
}
