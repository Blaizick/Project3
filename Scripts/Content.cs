using System.Collections.Generic;

public static class Blocks
{
    public static CmsEnt turret0;
    public static CmsEnt essenceCollector;

    public static List<CmsEnt> all;

    public static void Init()
    {
        turret0 = Cms.Get("Turret0");
        essenceCollector = Cms.Get("EssenceCollector");
        
        all = new()
        {
            turret0, essenceCollector
        };
    }
}


public static class CmsResources
{
    public static CmsEnt essence;

    public static List<CmsEnt> all;

    public static void Init()
    {
        essence = Cms.Get("Essence");

        all = new()
        {
            essence
        };
    }
}