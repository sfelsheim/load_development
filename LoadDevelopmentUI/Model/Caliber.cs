using SQLite;
using System;

namespace DataAccess.Model
{
    public class Caliber
    {
        public Caliber() { }
        /**
        Creates a new caliber and generates a new key
        */
        public Caliber(CaliberMaster cm)
        {
            this.caliberMaster = cm;
            this.ID = Guid.NewGuid();
            this.CaliberMasterID = cm.CaliberID;
        }
        [PrimaryKey]
        public Guid ID { get; set; }
        public Guid CaliberMasterID { get; set; }
        public Guid RifleID { get; set; }

        private CaliberMaster caliberMaster = null;

    }
}
