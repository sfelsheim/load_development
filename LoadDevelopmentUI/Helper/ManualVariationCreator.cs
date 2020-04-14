using System;
using DataAccess;
using DataAccess.Model;

namespace LoadDevelopmentUI.Helper
{
    public class ManualVariationCreator
    {
        private LoadDatabase database;

        public ManualVariationCreator(LoadDatabase database)
        {
            this.database = database;
        }

        public ManualVariation CreateManualVariation(Guid loadId, int numRounds, float coal, float powderCharge)
        {
            return new ManualVariation
            {
                LoadID = loadId,
                ManualVariationID = Guid.NewGuid(),
                Coal = coal,
                NumRounds = numRounds,
                PowderCharge = powderCharge
            };
	    }

        public LoadString CreateLoadString(ManualVariation mv, string id)
        {
            return new LoadString
            {
                LoadID = mv.LoadID,
                LoadStringID = mv.ManualVariationID,
                ID = id,
                NumRounds = mv.NumRounds,
                PowderCharge = mv.PowderCharge,
                Coal = mv.Coal,
                Variation = VariationType.Manual
            };
	    }
        public LoadString CreateManualLoadString(Guid loadId,
	                                      string id,
					                      int numRounds,
					                      float powderCharge,
					                      float coal)
        {

            Guid variationId = Guid.NewGuid();
            var mv = CreateManualVariation(loadId, numRounds, coal, powderCharge);
            database.InsertManualVariation(mv);

            return CreateLoadString(mv, id);
	    }
    }
}
