using System;
using SQLite;

namespace DataAccess.Model
{
    public class CaliberMaster : MasterDataBase
    {
        public CaliberMaster() { }

        public CaliberMaster(string name) 
        {
            this.Name = name;
            this.CaliberID = Guid.NewGuid();
        }

        [PrimaryKey]
        public Guid CaliberID { set; get; }
        public string Name { set; get; }

        public decimal BulletDiameter { set; get; }
        public override bool Equals(object obj)
        {
            return (obj is CaliberMaster && (obj as CaliberMaster).Name.Equals(this.Name));
        }
    }
}
