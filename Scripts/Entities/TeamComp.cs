using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamComp : MonoBehaviour
{
    public Team team;

    public void Set(Team team)
    {
        this.team = team;
    }
}

public class Team
{
    public CmsEnt cmsEnt;

    public string Name => cmsEnt.Get<CmsNameComp>().name;

    public bool IsEnemy(Team other)
    {
        return cmsEnt.
            GetAll<CmsEnemyTeamComp>().
            FindIndex(i => i.enemyTeam.tag == other.cmsEnt.tag) >= 0;
    }
    public bool IsAlly(Team other)
    {
        return cmsEnt.
            GetAll<CmsEnemyTeamComp>().
            FindIndex(i => i.enemyTeam.tag == other.cmsEnt.tag) < 0;
    }

    public List<Team> EnemyTeams
    {
        get
        {
            List<Team> teams = new();
            foreach (var i in cmsEnt.GetAll<CmsEnemyTeamComp>())
                teams.Add(Teams.Get(i.enemyTeam));                
            return teams;
        }
    }
}

[Serializable]
public class CmsEnemyTeamComp : CmsComp
{
    public CmsEnt enemyTeam;
}

public static class Teams
{
    public static Team ally;
    public static Team enemy;

    public static List<Team> all;

    public static void Init()
    {
        ally = new()
        {
            cmsEnt = Cms.Get("AllyTeam"),
        };
        enemy = new()
        {
            cmsEnt = Cms.Get("EnemyTeam"),
        };

        all = new()
        {
            ally, enemy,
        };
    }

    public static Team Get(CmsEnt cmsEnt)
    {
        return all.Find(i => i.cmsEnt.tag == cmsEnt.tag);
    }
}

[Serializable]
public class CmsNameComp : CmsComp
{
    public string name;
}