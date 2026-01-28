using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using BJect;
using JetBrains.Annotations;
using UnityEngine;

public class ImpsMain : MonoBehaviour
{
    public ResolverMain resolver;

    [Inject, NonSerialized] public DesktopInput input;
    [Inject, NonSerialized] public Player player;
    [Inject, NonSerialized] public EnemySpawner enemySpawner;
    [Inject, NonSerialized] public Ui ui; 
    [Inject, NonSerialized] public ResourcesSystem resources;

    public void Awake()
    {
        Cms.Init();
        Teams.Init();
        CmsResources.Init();
        Blocks.Init();

        resolver.Init();

        input.Init();

        resources.Init();
        player.Init();
        enemySpawner.Init();

        ui.Init();
    }

    public void Update()
    {
        resources.Update();
        enemySpawner.Update();
    }
}



public class ResourcesSystem
{
    public Dictionary<CmsEnt, float> resourcesDic = new();

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
}