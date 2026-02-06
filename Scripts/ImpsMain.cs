using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;
using BJect;
using JetBrains.Annotations;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class ImpsMain : MonoBehaviour
{
    public ResolverMain resolver;

    [Inject, NonSerialized] public DesktopInput input;
    [Inject, NonSerialized] public Player player;
    [Inject, NonSerialized] public EnemySpawner enemySpawner;
    [Inject, NonSerialized] public Ui ui; 
    [Inject, NonSerialized] public ResourcesSystem resources;
    [Inject, NonSerialized] public UnlocksSystem unlocks;
    [Inject, NonSerialized] public BuildingsSystem buildings;
    [Inject, NonSerialized] public CastlesSystem castles;
    [Inject, NonSerialized] public LayerMasks layerMasks;
    [Inject, NonSerialized] public CapturePointSystem capturePoints;
    [Inject, NonSerialized] public FogOfWarSystem fogOfWar;

    public void Awake()
    {
        Stopwatch sw = Stopwatch.StartNew();

        resolver.Init();

        Cms.Init();
        Teams.Init();
        CmsResources.Init();
        Blocks.Init();
        Profiles.Init();
        Castles.Init();
        layerMasks.Init();

        input.Init();

        buildings.Init();
        castles.Init();
        resources.Init();
        enemySpawner.Init();
        unlocks.Init();
        player.Init();
        capturePoints.Init();
        fogOfWar.Init();

        ui.Init();

        UnityEngine.Debug.Log($"Initialized in {sw.ElapsedMilliseconds} ms.");
    }

    public void Update()
    {
        resources.Update();
        enemySpawner.Update();
        castles.Update();
        buildings.Update();
    }
}

public class UnlocksSystem
{
    public List<CmsEnt> all;
    public HashSet<CmsEnt> unlocked;

    public UnityEvent onChange = new();

    public void Init()
    {
        all = new();
        unlocked = new();

        foreach (var i in Profiles.unlocksProfile.GetAll<CmsUnlockComp>())
        {
            all.Add(i.unlock);
            if (i.unlocked)
                unlocked.Add(i.unlock);
        }

        onChange?.Invoke();
    }

    public bool IsUnlocked(CmsEnt cmsEnt)
    {
        return unlocked.Contains(cmsEnt);
    }

    public void Unlock(CmsEnt cmsEnt)
    {
        if (!IsUnlocked(cmsEnt))
        {
            UnlockUnchecked(cmsEnt);
        }
    }
    public void UnlockUnchecked(CmsEnt cmsEnt)
    {
        unlocked.Add(cmsEnt);
        onChange?.Invoke();
    }

    public void Lock(CmsEnt cmsEnt)
    {
        unlocked.Remove(cmsEnt);
        onChange?.Invoke();
    }

    public List<CmsEnt> GetUnlockedBlocks()
    {
        List<CmsEnt> o = new();
        foreach (var i in unlocked)
        {
            if (BlockUtils.IsBlock(i))
            {
                o.Add(i);
            }
        }
        return o;
    }
}

[Serializable]
public class CmsUnlockComp : CmsComp
{
    public CmsEnt unlock;
    public bool unlocked;
}


[Serializable]
public class CmsStartResourceStackComp : CmsResourceStackComp
{
    
}