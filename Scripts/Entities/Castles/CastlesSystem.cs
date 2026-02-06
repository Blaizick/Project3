using System;
using System.Collections.Generic;
using System.Linq;
using BJect;
using UnityEngine;

[Serializable]
public class CmsMaxCastlesStoredForTeamComp : CmsComp
{
    public int maxCastlesStored;
    public CmsEnt team;
}
[Serializable]
public class CmsMaxCastlesForTeamComp : CmsComp
{
    public int maxCastles;
    public CmsEnt team;
}

public class CastlesSystem
{
    public List<Castle> all = new();
    public Dictionary<Team, TeamCastlesSystem> teamsDic = new();

    [Inject] public DiContainer container;

    public void Init()
    {
        var allMaxStored = Setups.Setup0.GetAll<CmsMaxCastlesStoredForTeamComp>();
        var allMax = Setups.Setup0.GetAll<CmsMaxCastlesForTeamComp>();

        foreach (var team in Teams.all)
        {
            teamsDic[team] = container.Create<TeamCastlesSystem>(new(){team});

            {
                var tmp = allMax.Find(i => i.team == team.cmsEnt);
                teamsDic[team].maxCastles = tmp != null ? tmp.maxCastles : teamsDic[team].maxCastles;
            }
            {
                var tmp = allMaxStored.Find(i => i.team == team.cmsEnt);
                teamsDic[team].maxCastlesStored = tmp != null ? tmp.maxCastlesStored : teamsDic[team].maxCastlesStored;
            }
            
            teamsDic[team].Init();
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
    public int count;

    [Inject] public CastlesSystem castles;
    [Inject] public DiContainer container;

    public Team team;

    public int castlesStored;
    public int maxCastlesStored;
    public int maxCastles;

    public TeamCastlesSystem(Team team)
    {
        this.team = team;
    }

    public void Init()
    {
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

    public AppearCastleGrid SpawnUncheckedWithAppearAnim(Vector2 pos, CmsEnt cmsEnt)
    {
        var scr = container.Instantiate(Profiles.basePrefabs.Get<CmsCastleGridAppearPfbComp>().pfb);
        scr.team = team;
        scr.transform.position = pos;
        scr.Set(cmsEnt);
        scr.Init();
        return scr;
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
        var scr = container.Instantiate(cmsEnt.Get<CmsCastlePfbComp>().pfb);
        scr.cmsEnt = cmsEnt;
        scr.teamComp.team = team;

        scr.transform.position = pos;

        scr.grid.Set(cmsEnt, scr);

        var shadow = container.Instantiate(Profiles.basePrefabs.Get<CmsShadowPfbComp>().shadowPfb, scr.transform);
        shadow.transform.localScale = new Vector3(scr.grid.size, scr.grid.size, 1.0f);
        shadow.transform.position = (Vector2)scr.transform.position - new Vector2(0.25f, 0.25f);

        Vector2 sizeV = new(scr.grid.size, scr.grid.size);
        scr.col.size = sizeV;
        scr.col.offset = Vector2.zero;
            
        scr.Init();
        scr.grid.Init();

        castles.Add(scr);
        foreach (var b in cmsEnt.GetAll<CmsCastlePlaceBlockComp>())
        {
            scr.grid.PlaceBlock(b.block, b.pos);
        }
        count++;

        return scr;
    }

    public bool CanPlaceOne() => castlesStored > 0 && count < maxCastles;

    public bool CanSpawn(Vector2 pos, CmsEnt cmsEnt)
    {
        return !Physics2D.OverlapBoxAll(pos, 
                new(cmsEnt.Get<CmsGridSizeComp>().size, 
                cmsEnt.Get<CmsGridSizeComp>().size), 0).
            ToList().
            Find(i => i.TryGetComponent<Castle>(out var c));
    }
}

[Serializable]
public class CmsGridPfbComp : CmsComp
{
    public BGrid gridPfb;
}

[Serializable]
public class CmsCastleGridAppearPfbComp : CmsComp
{
    public AppearCastleGrid pfb;
}