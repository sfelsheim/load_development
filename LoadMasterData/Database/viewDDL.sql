select 
    Load.LoadID,
    Load.Name as LoadName,
    Rifle.Name as RifleName,
    CaliberMaster.Name as CaliberName,
    PowderManufacturerMaster.Name as PowderManfName,
    PowderModelMaster.Name as PowderName,
    BulletManufacturerMaster.Name as BulletManfName,
    BulletModelMaster.Name as BulletName,
    PrimerManufacturerMaster.Name as PrimerManfName,
    PrimerModelMaster.Name as PrimerName,
    BrassMaster.Name as BrassName,
    Load.StartingPowderCharge,
    Load.StartingCOAL,
    Load.COAL,
    Load.ShotsPerVariation,
    Load.VaryByCOAL,
    Load.VaryByPowderCharge,
    Load.PowderVariations,
    Load.PowderVariationAmount,
    Load.PowderCharge,
    Load.CoalVariations,
    Load.COALVariationAmount,
    BulletModelMaster.Weight as BulletWeight
from Load
     LEFT OUTER JOIN Rifle on Load.RifleID = Rifle.ID
     LEFT OUTER JOIN CaliberMaster on Rifle.CaliberID = CaliberMaster.CaliberID
     LEFT OUTER JOIN PowderManufacturerMaster on Load.PowderManfID = PowderManufacturerMaster.PowderManufacturerMasterID
     LEFT OUTER JOIN PowderModelMaster on Load.PowderModelID = PowderModelMaster.PowderModelMasterID
     LEFT OUTER JOIN BulletManufacturerMaster on Load.BulletManfID = BulletManufacturerMaster.BulletManufacturerMasterID
     LEFT OUTER JOIN BulletModelMaster on Load.BulletModelID = BulletModelMaster.BulletModelID
     LEFT OUTER JOIN PrimerManufacturerMaster on Load.PrimerManfID = PrimerManufacturerMaster.PrimerManufacturerId
     LEFT OUTER JOIN PrimerModelMaster on Load.PrimerModelID = PrimerModelMaster.PrimerModelId
     LEFT OUTER JOIN BrassMaster on Load.CaseManfID = BrassMaster.BrassMasterId