
using System;
using BJect;
using UnityEngine;

public class CastleAssembler : Building
{
    [NonSerialized] public CmsEnt recipe;

    [NonSerialized] public bool crafting;
    [NonSerialized] public float progress;

    [Inject, NonSerialized] public ResourcesSystem resources;
    [Inject, NonSerialized] public CastleAssemblerUi castleAssemblerUi;
    [Inject, NonSerialized] public CastlesSystem castles;

    public override void Init()
    {
        recipe = cmsEnt.Get<CmsRecipeComp>().recipe;
        
        base.Init();
    }

    public override void Update()
    {
        if (crafting)
        {
            progress += Time.deltaTime / recipe.Get<CmsCraftTimeComp>().time;
            if (progress > 1)
            {
                progress = 0.0f;
                crafting = false;
                castles.ForTeam(teamComp.team).castlesStored++;
            }
        }

        busy = crafting;

        base.Update();
    }

    public bool CanStartCrafting()
    {
        if (crafting)
        {
            return false;
        }
        var c = castles.ForTeam(teamComp.team);
        if (c.castlesStored >= c.maxCastlesStored)
        {
            return false;
        }
        foreach (var i in recipe.GetAll<CmsInComp>())
        {
            if (!resources.Has(i.AsStack()))
            {
                return false;
            }
        }
        return true; 
    }

    public void StartCraftingUnchecked()
    {
        crafting = true;
        foreach (var i in recipe.GetAll<CmsInComp>())
        {
            resources.Remove(i.AsStack());
        }
        progress = 0.0f;
    }

    public void StartCrafting()
    {
        if (CanStartCrafting())
        {
            StartCraftingUnchecked();
        }
    }

    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     castleAssemblerUi.Show(() =>
    //     {
    //         castles.StartPlacing(Castles.playerCastle0);
    //     }, () =>
    //     {
    //         if (CanStartCrafting())
    //         {
    //             StartCrafting();
    //         }
    //     });
    // }
}

[Serializable]
public class CmsMaxCastlesComp : CmsComp
{
    public int max;
}

[Serializable]
public class CmsMaxCastlesStoredComp : CmsComp
{
    public int maxStored;
}