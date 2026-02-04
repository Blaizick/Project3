using System;
using System.Collections.Generic;
using System.Linq;
using BJect;
using UnityEngine;

public class CastlesSystem
{
    public List<Castle> all = new();
    public Dictionary<Team, TeamCastlesSystem> teamsDic = new();

    [Inject] public DiContainer container;

    public void Init()
    {
        foreach (var team in Teams.all)
        {
            teamsDic[team] = container.Create<TeamCastlesSystem>(new(){team});
        }
    }

    public void Update()
    {
        all.RemoveAll(i => i == null);
        foreach (var (k, v) in teamsDic)
        {
            v.Update();
        }
    }

    public void Add(Castle castle)
    {
        all.Add(castle);
        ForTeam(castle.teamComp.team).all.Add(castle);
    }
    public void Remove(Castle castle)
    {
        all.Remove(castle);
        ForTeam(castle.teamComp.team).all.Remove(castle);
    }

    public BGrid SpawnSCastle(CmsEnt cmsEnt)
    {
        var p = container.Instantiate(Profiles.playerCastlesProfile.Get<CmsGridPfbComp>().gridPfb);
        p.Set(cmsEnt);
        p.Init();
        return p;
    }

    public TeamCastlesSystem ForTeam(Team team)
    {
        return teamsDic[team];
    }
}

public class TeamCastlesSystem
{
    public List<Castle> all = new();

    [Inject] public Castle.Factory castlesFactory;
    [Inject] public CastlesSystem castles;

    public Team team;
    public CmsEnt cmsEnt;

    public int castlesStored;
    public int maxCastlesStored;
    public int maxCastles;

    public TeamCastlesSystem(Team team)
    {
        this.team = team;
        cmsEnt = team.cmsEnt;
    }

    public void Init()
    {
        maxCastlesStored = cmsEnt.Get<CmsMaxCastlesStoredComp>().maxStored;
        maxCastles = cmsEnt.Get<CmsMaxCastlesComp>().max;
        
        castlesStored = 0;
    }
    public void Update()
    {
        all.RemoveAll(i => i == null);
    }

    public void Set(Team team)
    {
        this.team = team;
    }

    public Castle Spawn(Vector2 pos, CmsEnt cmsEnt)
    {
        if (CanSpawn(pos, cmsEnt))
        {
            return SpawnUnchecked(pos, cmsEnt);
        }
        return null;
    }

    public Castle SpawnUnchecked(Vector2 pos, CmsEnt cmsEnt)
    {
        var scr = castlesFactory.Use(pos, cmsEnt, team);
        castles.Add(scr);
        foreach (var b in cmsEnt.GetAll<CmsCastlePlaceBlockComp>())
        {
            scr.grid.PlaceBlock(b.block, b.pos);
        }
        return scr;
    }

    public bool HasCastle() => castlesStored > 0;

    public bool CanPlaceOne() => castlesStored > 0 && all.Count < maxCastles;

    public bool CanSpawn(Vector2 pos, CmsEnt cmsEnt)
    {
        var hits = Physics2D.OverlapCircleAll(pos, cmsEnt.Get<CmsGridSizeComp>().size);
        if (hits != null && hits.Count() > 0)
        {
            return false;
        }
        return true;
    }
}

[Serializable]
public class CmsGridPfbComp : CmsComp
{
    public BGrid gridPfb;
}
