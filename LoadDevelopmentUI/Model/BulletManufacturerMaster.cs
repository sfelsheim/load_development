using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace DataAccess.Model
{
    public class BulletManufacturerMaster : MasterDataBase
    {
        public BulletManufacturerMaster() { }

        public BulletManufacturerMaster(string manufacturer, Guid guid)
        {
            this.Name = manufacturer;
            this.BulletManufacturerMasterID = guid;
        }

        [PrimaryKey]
        public Guid BulletManufacturerMasterID { set; get; }

        public string Name { set; get; }

        public override bool Equals(object obj)
        {
            if (obj is BulletManufacturerMaster)
                return BulletManufacturerMasterID.Equals((obj as BulletManufacturerMaster).BulletManufacturerMasterID);
            return false;
        }
    }
}
