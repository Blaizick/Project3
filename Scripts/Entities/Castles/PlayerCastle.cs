using System;
using BJect;

public class PlayerCastle : Castle
{
    [Inject, NonSerialized] public DiContainer container;
    [Inject, NonSerialized] public FogOfWarSystem fogOfWar;

    public CraftSystem craftSystem;

    public override void Init()
    {
        craftSystem = container.Create<CraftSystem>(new(){teamComp.team});
        craftSystem.recipe = cmsEnt.Get<CmsRecipeComp>().recipe;

        base.Init();
    }

    public override void Update()
    {
        craftSystem.Update();
        fogOfWar.RevealWorld(transform.position, cmsEnt.Get<CmsRevealRangeComp>().revealRange);
        base.Update();
    }
}

[Serializable]
public class CmsRevealRangeComp : CmsComp
{
    public float revealRange;
}