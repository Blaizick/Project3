using BJect;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner
{
    public CastlesSystem castles;

    public float spawnProgress;

    public CmsEnt cmsEnt;
    public Team team;

    public CmsEnemySpawnerComp curSpawnComp;
    public List<CmsEnemySpawnerComp> spawnComps;
    public List<CmsEnemySpawnEventComp> spawnEventComps;
    public HashSet<CmsEnemySpawnEventComp> invokedEvents = new();
    public CmsEnemySpawnerComp lastSpawnerComp;

    public MapSystem map;

    public float TimeLeft => lastSpawnerComp.timeNeeded - Time.time; 

    public EnemySpawner(CastlesSystem castles, MapSystem map)
    {
        this.castles = castles;
        this.map = map;
    }

    public void Init()
    {
        cmsEnt = Profiles.enemySpawner0;
        
        team = Teams.Get(cmsEnt.Get<CmsTeamComp>().team);

        spawnComps = cmsEnt.GetAll<CmsEnemySpawnerComp>();
        spawnEventComps = cmsEnt.GetAll<CmsEnemySpawnEventComp>();
    
        lastSpawnerComp = null;
        foreach (var i in spawnComps)
        {
            if (lastSpawnerComp == null || i.timeNeeded > lastSpawnerComp.timeNeeded)
            {
                lastSpawnerComp = i;
            }
        }
    }

    public void Update()
    {
        var castlesL = castles.ForTeam(team.EnemyTeams.First()).all;
        if (castlesL.Count <= 0)
        {
            return;
        }

        curSpawnComp = null;
        foreach (var spawnComp in spawnComps)
        {
            if (Time.time > spawnComp.timeNeeded)
            {
                if (curSpawnComp == null || spawnComp.timeNeeded > curSpawnComp.timeNeeded)
                {
                    curSpawnComp = spawnComp;
                }
            }
        }
        if (curSpawnComp == null)
        {
            return;
        }

        var targetCastle = castlesL[UnityEngine.Random.Range(0, castlesL.Count)];

        foreach (var e in spawnEventComps)
        {
            if (Time.time > e.timeNeeded)
            {
                if (!spawnEventComps.Contains(e))
                {
                    foreach (var c in e.castles)
                    {
                        if (!GetSpawnPos(targetCastle, e.minSpawnDst, e.maxSpawnDst, out var spawnPos))
                        {
                            continue;
                        }
                        var castle = e.castles[UnityEngine.Random.Range(0, e.castles.Count)];
                        castles.ForTeam(team).SpawnUnchecked(spawnPos, castle);
                    }
                    spawnEventComps.Add(e);
                }
            }
        }

        spawnProgress += Time.deltaTime / curSpawnComp.spawnDelay;

        if (spawnProgress > 1.0f)
        {
            if (!GetSpawnPos(targetCastle, curSpawnComp.minSpawnDst, curSpawnComp.maxSpawnDst, out var spawnPos))
            {
                return;
            }

            var castle = curSpawnComp.castles[UnityEngine.Random.Range(0, curSpawnComp.castles.Count)];
            castles.ForTeam(team).SpawnUncheckedWithAppearAnim(spawnPos, castle);

            spawnProgress = 0.0f;
        }
    }

    public bool GetSpawnPos(Castle castle, float min, float max, out Vector2 ans)
    {
        float hMax = max * 0.5f;
        Vector2 hMaxV = new Vector2(hMax, hMax);

        Vector2 aa = (Vector2)castle.grid.transform.position - hMaxV;
        Vector2 bb = (Vector2)castle.grid.transform.position + hMaxV;

        int it = 0;
        do
        {
            ans = new Vector2(UnityEngine.Random.Range(aa.x, bb.x), UnityEngine.Random.Range(aa.y, bb.y));

            if (it++ > 50)
            {
                return false;
            }
        }
        while (Vector2.Distance(ans, castle.grid.transform.position) < min || !map.IsPositionInBounds(ans));
        return true;        
    }
}

[Serializable]
public class CmsTeamComp : CmsComp
{
    public CmsEnt team;
}

[Serializable]
public class CmsEnemySpawnerComp : CmsComp
{
    public float spawnDelay;
    public float timeNeeded;
    public float minSpawnDst;
    public float maxSpawnDst;
    public List<CmsEnt> castles = new();
}

[Serializable]
public class CmsEnemySpawnEventComp : CmsComp
{
    public float timeNeeded;
    public float minSpawnDst;
    public float maxSpawnDst;
    public List<CmsEnt> castles = new();
}

[Serializable]
public class CmsCastlePlaceBlockComp : CmsComp
{
    public CmsEnt block;
    public Vector2Int pos;
}

[Serializable]
public class CmsMinMaxDstComp : CmsComp
{
    public float minDst;
    public float maxDst;
}

[Serializable]
public class CmsAgreRangeComp : CmsComp
{
    public float range;
}