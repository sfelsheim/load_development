using System;
using SQLite;
namespace DataAccess.Model
{
    public class BulletMasterProfile
    {
        [PrimaryKey]
        public Guid BulletMasterProfileID { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Caliber { get; set; }
        public string Weight { get; set; }
        public bool IsReady()
        {
            //TODO 
            return true;
		}
        public void Clear()
        { 
			//TODO
		}
    }
}
