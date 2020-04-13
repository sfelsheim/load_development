using SQLite;
using System;

namespace DataAccess.Model
{
    public class Load
    {
        static public string DEVELOP_STATE = "D";

        static public string TEST_STATE = "T";

        static public string USE_STATE = "U";

        [PrimaryKey]
        public Guid LoadID { set; get; }

        public string Name { set; get; }

        public Guid RifleID { set; get; }
        public Guid PowderManfID { set; get; }
        public Guid PowderModelID { set; get; }
        public Guid BulletManfID { set; get; }
        public Guid BulletModelID { set; get; }
        public Guid PrimerManfID { get; set; }
        public Guid PrimerModelID { get; set; }
        public Guid CaseManfID { get; set; }
        public float CaseOAL { get; set; }
        public float CaseHeadspace { get; set; }
        public bool VaryByPowderCharge { get; set; } = true;
        public bool VaryByCOAL { get; set; } = false;
        public bool VaryManually { get; set; } = false;
        public int PowderVariations { get; set; } = 10;
        public int CoalVariations { get; set; } = 5;
        public int ManualVariations { get; set; }
        public int ShotsPerVariation { get; set; } = 3;
        public float PowderVariationAmount { get; set; } = 0.03F;
        public float StartingPowderCharge { set; get; } = -1F;
        public float COAL { get; set; } = -1F;
        public float COALVariationAmount { get; set; } = 0.002F;
        public float StartingCOAL { get; set; } = -1F;
        public float PowderCharge { get; set; } = -1F;
        public Guid BulletWeightID { get; set; }
        [MaxLength(1)]
        public string LoadState { get; set; } = DEVELOP_STATE;
        public int FowlingRounds { set; get; } = 3;
        public bool IsNew { set; get; } = true;
    }
}
