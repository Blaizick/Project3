using System;
using BJect;

public class PlayerCastle : Castle
{
    [Inject, NonSerialized] public Container container;

    public CraftSystem craftSystem;

    public override void Init()
    {
        craftSystem = container.Create<CraftSystem>();
        craftSystem.recipe = cmsEnt.Get<CmsRecipeComp>().recipe;

        base.Init();
    }

    public override void Update()
    {
        craftSystem.Update();
        base.Update();
    }
}