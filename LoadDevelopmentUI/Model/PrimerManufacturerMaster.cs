using System;
using SQLite;
namespace DataAccess.Model
{
    public class PrimerManufacturerMaster : MasterDataBase
    {
        [PrimaryKey]
        public Guid PrimerManufacturerId { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is PrimerManufacturerMaster)
                return PrimerManufacturerId.Equals((obj as PrimerManufacturerMaster).PrimerManufacturerId);
            return false;
        }
    }
}
