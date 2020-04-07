using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace DataAccess.Model
{
    public class Bullet
    {
        [PrimaryKey]
        public Guid BulletID { set; get; }
        public Guid BulletManufacturerMasterID { set; get; }
        public Guid BulletModelMasterID { set; get; }
    }
}
