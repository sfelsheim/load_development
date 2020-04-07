using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Model
{
    public class PowderManufacturerMaster : MasterDataBase
    {
        public PowderManufacturerMaster() {}
        public PowderManufacturerMaster(string name) 
        {
            this.PowderManufacturerMasterID = Guid.NewGuid();
            this.Name = name;
        }

        [PrimaryKey]
        public Guid PowderManufacturerMasterID { set; get; }

        public string Name { set; get; }

        public override bool Equals(object obj)
        {
            return PowderManufacturerMasterID.Equals((obj as PowderManufacturerMaster).PowderManufacturerMasterID); 
        }
    }
}
