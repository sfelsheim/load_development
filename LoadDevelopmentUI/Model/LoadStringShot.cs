using System;
using SQLite;
namespace DataAccess.Model
{
    public class LoadStringShot
    {
        [PrimaryKey]
        public Guid LoadStringShotId {get; set;}
        public Guid LoadStringId { get; set; }
        public Guid LoadId { get; set; }
        public int Velocity { get; set; } = 0;
        public bool IsError { get; set; } = false;
        [Ignore]
        public string VelocityStr
        {
            get 
			{
                return Velocity <= 0 ? null : Velocity.ToString();
			}

            set
            {
                var intVal = 0;
                if (int.TryParse(value, out intVal))
                    Velocity = intVal;
                else
                    Velocity = 0;
			}
		}

        [Ignore]
        public string ID 
		{ 
            get { return TheLoadString.ID; }
		}

        [Ignore]
        public float PowderCharge
        { 
            get { return TheLoadString.PowderCharge;  }
		}

        [Ignore]
        public float Coal
        { 
            get { return TheLoadString.Coal;  }
		}

        [Ignore]
        public string Detail
        {
            get {
                return string.Format("ID: {0}, Charge: {0:F1}, COAL: {0:F3}",
                    ID, PowderCharge, Coal);
				}
		}
        [Ignore]
        public LoadString TheLoadString { get; set; }
    }
}
