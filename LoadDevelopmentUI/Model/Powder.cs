using SQLite;
using System;

namespace DataAccess.Model
{
    public class Powder
    {
        [PrimaryKey]
        public Guid PowderID { set; get; }

        public Guid PowderManufacturerID { set; get; }
        public Guid PowderModelID { set; get; }
    }
}