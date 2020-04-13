using System;
using SQLite;
namespace DataAccess.Model
{
    public class LoadDisplay
    {
        public Guid LoadID { get; set; }
        public string LoadName { get; set; }
        public string RifleName { get; set; }
        public string CaliberName { get; set; }
        public string PowderManfName { get; set; }
        public string PowderName { get; set; }
        public string BulletManfName { get; set; }
        public string BulletName { get; set; }
        public string PrimerManfName { get; set; }
        public string PrimerName { get; set; }
        public string BrassName { get; set; }
        public float StartingPowderCharge { get; set; }
        public float StartingCOAL { get; set; }
        public float COAL { get; set; }
        public int ShotsPerVariation { get; set; }
        public bool VaryByCOAL { get; set; }
        public bool VaryByPowderCharge { get; set; }
        public bool VaryByManual { get; set; }
        public int PowderVariations { get; set; }
        public float PowderVariationAmount { get; set; }
        public float PowderCharge { get; set; }
        public int CoalVariations { get; set; }
        public float COALVariationAmount { get; set; }
        public string BulletWeight { get; set; }
        public string LoadState { get; set; } = Load.DEVELOP_STATE;
        public int ManualVariations { get; set; }

        [Ignore]
        public string ListTitle
        {
            get
            {
                return string.IsNullOrEmpty(LoadName) ? "Unnamed load" :  LoadName + " - " + RifleLabel;
            }
        }
        [Ignore]
        public string RifleLabel
        {
            get 
            {
                if (string.IsNullOrEmpty(RifleName))
                    return "Unknown Rifle";

                return RifleName + " (" + CaliberName + ")";
            }
        }

        [Ignore]
        public string BulletLabel
        {
            get
            {
                return BulletManfName + " " + BulletName + " " + BulletWeight + "gr";
            }
        }

        [Ignore]
        public string PowderLabel
        {
            get
            {
                return PowderManfName + " " + PowderName;
            }
        }

        [Ignore]
        public string VariationsLabel
        {
            get
            {
                if (VaryByPowderCharge)
                    return PowderVariations + " powder variations, starting at " + StartingPowderCharge.ToString("F1") + "gr";
                else if (VaryByCOAL)
                    return CoalVariations + " COAL variations, starting at " + StartingCOAL.ToString("F3") + " varying by " + COALVariationAmount.ToString("F3");
                else
                    return ManualVariations + " manual variations.";
            }
        }
    }
}
