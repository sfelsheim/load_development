using System;
using SQLite;
namespace DataAccess.Model
{
    public class BrassMaster : MasterDataBase
    {
        [PrimaryKey]
        public Guid BrassMasterId { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is BrassMaster)
                return BrassMasterId.Equals((obj as BrassMaster).BrassMasterId);
            return false;
        }
    }
}
