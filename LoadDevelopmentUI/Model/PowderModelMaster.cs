using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace DataAccess.Model
{
    public class PowderModelMaster : MasterDataBase
    {
        public PowderModelMaster() { }
        public PowderModelMaster(string name, Guid manfId) 
        {
            this.Name = name;
            this.PowderManufacturerMasterID = manfId;
            this.PowderModelMasterID = Guid.NewGuid();
        }

        [PrimaryKey]
        public Guid PowderModelMasterID { set; get; }
        public Guid PowderManufacturerMasterID { set; get; }
        public string Name { set; get; }

        public override bool Equals(object obj)
        {
            if (obj is PowderModelMaster)
                return PowderModelMasterID.Equals((obj as PowderModelMaster).PowderModelMasterID);
            return false;
        }
    }
}
