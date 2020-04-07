using SQLite;
using System;

namespace DataAccess.Model
{
    public class BulletModelMaster : MasterDataBase
    {
        public BulletModelMaster() { }

        public BulletModelMaster(Guid id, Guid currentManfID, string model, string caliber, string weight)
        {
            this.BulletModelID = id;
            this.BulletManufacturerID = currentManfID;
            this.Name = model;
            this.Diameter = decimal.Parse(caliber);
            this.Weight = weight;
        }

        [PrimaryKey]
        public Guid BulletModelID { get; set; }

        public Guid BulletManufacturerID { get; set; }

        public decimal Diameter { get; set; }
        public string Name{ get; set; }

        public string Weight { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is BulletModelMaster)
                return BulletModelID.Equals((obj as BulletModelMaster).BulletModelID);
            return false;
        }
    }
}