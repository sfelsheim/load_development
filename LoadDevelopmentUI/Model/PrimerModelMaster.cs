using System;
using SQLite;
namespace DataAccess.Model
{
    public class PrimerModelMaster : MasterDataBase
    {
        [PrimaryKey]
        public Guid PrimerModelId{ get; set; }
        public Guid PrimerManfId { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is PrimerModelMaster)
                return PrimerModelId.Equals((obj as PrimerModelMaster).PrimerModelId);
            return false;
        }
    }
}
