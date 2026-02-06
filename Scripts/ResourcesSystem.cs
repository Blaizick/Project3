using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesSystem
{
    public Dictionary<Team, TeamResourcesSystem> teamsDic = new();

    public void Init()
    {
        foreach (var t in Teams.all)
        {
            teamsDic[t] = new(t, this);
        }

        foreach (var s in Setups.Setup0.GetAll<CmsStartResourceStackComp>())
        {
            ForTeam(Teams.ally).Add(s.AsStack());
        }
    }

    public void Update()
    {
        foreach (var (k, v) in teamsDic)
        {
            v.Update();
        }
    }

    public TeamResourcesSystem ForTeam(Team team)
    {
        return teamsDic[team];
    }
}

public class TeamResourcesSystem
{
    public Team team;
    public ResourcesSystem resources;
    public Dictionary<CmsEnt, float> resourcesDic = new();

    public TeamResourcesSystem(Team team, ResourcesSystem resources)
    {
        this.team = team;
        this.resources = resources;
    }

    public void Init()
    {
        Add(CmsResources.essence, 50);
    }

    public void Update()
    {
        
    }

    public void Add(ResourceStack stack)
    {
        if (stack == null) return;
        Add(stack.resource, stack.count);
    }
    public void Add(CmsEnt res, float c)
    {
        if (!resourcesDic.ContainsKey(res))
        {
            resourcesDic[res] = 0;
        }
        resourcesDic[res] += c;
    }


    public void Remove(List<ResourceStack> stacks)
    {
        foreach (var stack in stacks)
        {
            Remove(stack);
        }
    }
    public void Remove(ResourceStack stack)
    {
        Remove(stack.resource, stack.count);
    }
    public void Remove(CmsEnt res, float c)
    {
        if (!resourcesDic.ContainsKey(res))
        {
            return;
        }
        resourcesDic[res] = Mathf.Clamp(resourcesDic[res] - c, 0, float.MaxValue);
    }

    public float Get(CmsEnt res)
    {
        return resourcesDic.TryGetValue(res, out var f) ? f : 0;
    }

    public bool Has(ResourceStack stack)
    {
        return Get(stack.resource) >= stack.count;
    }
    public bool Has(List<ResourceStack> stacks)
    {
        foreach (var stack in stacks)
        {
            if (!Has(stack))
            {
                return false;
            }
        }
        return true;
    }

    public bool InfinityResources => team.cmsEnt.Has<CmsInfinityResourcesTag>(); 
}

[Serializable]
public class CmsInfinityResourcesTag : CmsComp
{
    
}