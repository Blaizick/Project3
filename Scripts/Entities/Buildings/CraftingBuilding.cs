using System;
using System.Collections.Generic;
using BJect;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class ResourceStack : IFormattable
{
    public CmsEnt resource;
    public float count;

    public ResourceStack(CmsEnt res, float c)
    {
        resource = res;
        count = c;
    }

    public ResourceStack Deserialize()
    {
        return new(Cms.Get(resource.tag), count);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return $"{resource.Get<CmsNameComp>().name}: {count}";
    }
    public override string ToString()
    {
        return ToString(null, null);        
    }
}

[Serializable]
public class CmsResourceStackComp : CmsComp
{
    public CmsEnt resource;
    public float count;

    public ResourceStack AsStack()
    {
        return new(Cms.Get(resource.tag), count);
    }
}

public static class ResourcesUtils
{
    public static List<ResourceStack> GetAllStacks<T>(CmsEnt cmsEnt) where T : CmsResourceStackComp
    {
        List<ResourceStack> l = new();
        foreach (var r in cmsEnt.GetAll<T>())
        {
            l.Add(r.AsStack());
        }
        return l;
    }
}

[Serializable]
public class CmsInComp : CmsResourceStackComp {}

[Serializable]
public class CmsOutComp : CmsResourceStackComp {}

[Serializable]
public class CmsCraftTimeComp : CmsComp
{
    public float time;
}

[Serializable]
public class CmsRecipeComp : CmsComp
{
    public CmsEnt recipe;
}

public class CraftingBuilding : Building
{
    [NonSerialized] public CmsEnt recipe;

    [Inject, NonSerialized] public ResourcesSystem resources;
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


public class CraftSystem
{
    public ResourcesSystem resources; 

    public CmsEnt recipe;

    public bool crafting;
    public float craftProgress;

    public CraftSystem(ResourcesSystem resources)
    {
        this.resources = resources;
    }

    public void Update()
    {
        if (crafting)
        {
            craftProgress += Time.deltaTime / recipe.Get<CmsCraftTimeComp>().time;
            if (craftProgress > 1.0f)
            {
                foreach (var output in recipe.GetAll<CmsOutComp>())
                {
                    resources.Add(output.AsStack());
                }
                crafting = false;
            }
        }
        else
        {
            craftProgress = 0.0f;
            crafting = true;
            foreach (var input in recipe.GetAll<CmsInComp>())
            {
                if (!resources.Has(input.AsStack()))
                {
                    crafting = false;
                    break;
                }
            }
        }
    }
}