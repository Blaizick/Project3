using System;
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
        return other != this;
    }
    public bool IsAlly(Team other)
    {
        return other == this;
    }
}

public static class Teams
{
    public static Team ally;
    public static Team enemy;

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
    }
}

[Serializable]
public class CmsNameComp : CmsComp
{
    public string name;
}