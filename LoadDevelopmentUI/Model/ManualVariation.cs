using System;
using SQLite;
namespace DataAccess.Model
{
    public class ManualVariation
    {
        [PrimaryKey]
        public Guid ManualVariationID { get; set; }
        public Guid LoadID { get; set; }
        public int NumRounds { get; set; }
        public float PowderCharge { get; set; }
        public float Coal { get; set; }
    }
}
