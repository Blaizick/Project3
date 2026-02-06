using System.Collections.Generic;

public static class Blocks
{
    public static CmsEnt turret0;
    public static CmsEnt turret1;
    public static CmsEnt turret2;
    public static CmsEnt essenceCollector;
    public static CmsEnt mender;
    public static CmsEnt quintessenceCollector;
    public static CmsEnt lab;
    public static CmsEnt castleAssembler;
    public static CmsEnt forceField;
    public static CmsEnt constructBuilding;

    public static List<CmsEnt> all;

    public static void Init()
    {
        turret0 = Cms.Get("Turret0");
        turret1 = Cms.Get("Turret1");
        turret2 = Cms.Get("Turret2");
        essenceCollector = Cms.Get("EssenceCollector");
        quintessenceCollector = Cms.Get("QuintessenceCollector");
        mender = Cms.Get("Mender");
        lab = Cms.Get("Lab");
        castleAssembler = Cms.Get("CastleAssembler");
        forceField = Cms.Get("ForceField");
        constructBuilding = Cms.Get("ConstructBuilding");

        all = new()
        {
            turret0, turret1, turret2, 
            essenceCollector, quintessenceCollector, 
            mender, forceField,
            lab,
            castleAssembler,
            constructBuilding,
        };
    }
}

public static class CmsResources
{
    public static CmsEnt essence;
    public static CmsEnt quintessence;

    public static List<CmsEnt> all;

    public static void Init()
    {
        essence = Cms.Get("Essence");
        quintessence = Cms.Get("Quintessence");

        all = new()
        {   
            essence, quintessence
        };
    }
}

public static class Profiles
{
    public static CmsEnt unlocksProfile;
    public static CmsEnt playerCastlesProfile;
    public static CmsEnt enemySpawner0;
    public static CmsEnt capturePoints;
    public static CmsEnt basePrefabs;
    public static CmsEnt fogOfWar;

    public static void Init()
    {
        unlocksProfile = Cms.Get("UnlocksProfile");
        playerCastlesProfile = Cms.Get("PlayerCastlesProfile");
        enemySpawner0 = Cms.Get("EnemySpawner0");
        capturePoints = Cms.Get("CapturePoints0");
        basePrefabs = Cms.Get("BasePrefabs");
        fogOfWar = Cms.Get("FogOfWar");
    }
}

public static class Setups
{
    public static CmsEnt Setup0 => Cms.Get("Setup0");
}

public static class Castles
{
    public static CmsEnt playerCastle0;
    public static CmsEnt enemyCastle0;

    public static void Init()
    {
        playerCastle0 = Cms.Get("PlayerCastle0");
        enemyCastle0 = Cms.Get("EnemyCastle0");
    }
}