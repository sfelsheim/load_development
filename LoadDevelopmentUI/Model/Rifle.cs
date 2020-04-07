using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Model
{
    public class Rifle
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public string Name { get; set; }

        public Guid CaliberID { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Rifle)
                return (obj as Rifle).Name.Equals(this.Name);
            return false;
        }
    }
}
