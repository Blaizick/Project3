
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
        bool hasAll = true;
        if (!resources.ForTeam(teamComp.team).InfinityResources)
        {
            foreach (var i in recipe.GetAll<CmsInComp>())
            {
                if (!resources.ForTeam(teamComp.team).Has(i.AsStack()))
                {
                    hasAll = false;
                    break;
                }
            }    
        }
        return hasAll; 
    }

    public void StartCraftingUnchecked()
    {
        crafting = true;
        if (!resources.ForTeam(teamComp.team).InfinityResources)
        {
            foreach (var i in recipe.GetAll<CmsInComp>())
            {
                resources.ForTeam(teamComp.team).Remove(i.AsStack());
            }    
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