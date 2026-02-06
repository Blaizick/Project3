using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using BJect;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class CmsResourceRequirementComp : CmsResourceStackComp
{
    
}

public class Building : MonoBehaviour
{
    [NonSerialized] public CmsEnt cmsEnt;

    [NonSerialized] public Vector2Int pos;
    [NonSerialized] public CastleGrid grid;

    public HealthComp healthComp;
    public TeamComp teamComp;

    public TooltipTrigger tooltipTrigger;

    public BoxCollider2D col;

    public GameObject controlFrameRoot;

    public int Size => cmsEnt.Get<CmsGridSizeComp>().size;

    [Inject, NonSerialized] public UnlocksSystem unlocks;
    [Inject, NonSerialized] public DesktopInput input;

    public ActivityFlag activityFlag;

    [NonSerialized] public bool busy;

    public virtual void Init()
    {
        healthComp.Set(cmsEnt);
        healthComp.Init();
        healthComp.onDie.AddListener(() =>
        {
            grid.BreakBuild(this);
        });

        Unlock();

        if (controlFrameRoot)
        {
            controlFrameRoot.SetActive(false);
        }
        if (activityFlag)
        {
            activityFlag.transform.position = (Vector2)transform.position + 
                (new Vector2(Size, -Size) * 0.5f) + 
                new Vector2(-0.2f, 0.3f);
        }
    }

    public virtual void Unlock()
    {
        foreach (var i in cmsEnt.GetAll<CmsUnlockOnSpawnComp>())
        {
            unlocks.Unlock(i.unlock);
        }
    }

    public virtual void Update()
    {
        if (activityFlag.root)
        {
            activityFlag.busyState.root.SetActive(busy);
            activityFlag.idleState.root.SetActive(!busy);
        }

        healthComp.canDamage = true;
        healthComp.Update();

        if (tooltipTrigger)
        {
            tooltipTrigger.title = TooltipTitle;
            tooltipTrigger.desc = Desc;    
        }
    }

    public virtual void OnDestroy()
    {
    }

    public virtual bool CanBreak()
    {
        return true;
    }

    public virtual string TooltipTitle => BlockUtils.GetTooltipTitle(cmsEnt);
    public virtual string Desc => $"{BlockUtils.GetDesc(cmsEnt)}\n{AdditionalDescInfo}"; 
    public virtual string AdditionalDescInfo => $"Health: {(int)healthComp.health}/{(int)healthComp.maxHealth}\n" + 
                                                $"Team: {teamComp.team.cmsEnt.Get<CmsNameComp>().name}\n";
}

[Serializable]
public class CmsBuildPfbComp : CmsComp
{
    public Building pfb;
}

public static class BlockUtils
{
    public static string GetTooltipTitle(CmsEnt cmsEnt)
    {
        return cmsEnt.Get<CmsNameComp>().name;
    }
    public static string GetDesc(CmsEnt cmsEnt)
    {
        StringBuilder sb = new();

        sb.Append(cmsEnt.Get<CmsDescComp>().desc);

        return sb.ToString();
    }
    
    public static string GetBuildDesc(CmsEnt cmsEnt)
    {
        return $"{GetDesc(cmsEnt)}\n{BuildCostAsStr(cmsEnt)}"; 
    }

    public static string BuildCostAsStr(CmsEnt cmsEnt)
    {
        StringBuilder sb = new();

        var reqs = cmsEnt.GetAll<CmsResourceRequirementComp>();
        if (reqs.Count > 0)
        {
            sb.Append("Build Cost:\n");
            foreach (var r in reqs)
            {
                sb.Append($"{r.AsStack()}\n");
            }   
        }
        if (cmsEnt.TryGet<CmsRecipeComp>(out var recipeComp))
        {
            var recipe = recipeComp.recipe;
            sb.Append($"\nRecipe: {recipe.Get<CmsNameComp>().name}\n");

            var input = recipe.GetAll<CmsInComp>();
            var output = recipe.GetAll<CmsOutComp>();
            if (input.Count > 0)
            {
                sb.Append($"Input: \n");
                foreach (var i in input)
                {
                    sb.Append($"{i.AsStack()}\n");
                }
            }
            if (output.Count > 0)
            {
                sb.Append($"Output: \n");
                foreach (var i in output)
                {
                    sb.Append($"{i.AsStack()}\n");
                }
            }

            sb.Append($"Craft Time: {recipe.Get<CmsCraftTimeComp>().time}\n");
        }

        return sb.ToString();
    }

    public static bool IsBlock(CmsEnt cmsEnt)
    {
        return Blocks.all.Contains(cmsEnt);
    }
}

[Serializable]
public class CmsDescComp : CmsComp
{
    public string desc;
}

[Serializable]
public class CmsUnlockOnSpawnComp : CmsComp
{
    public CmsEnt unlock;
}

public class BuildingsSystem
{
    public List<Building> all = new();
    public Dictionary<Team, TeamBuildingsSystem> teamsDic = new();
    [Inject] public DiContainer container;

    public void Init()
    {
        foreach (var team in Teams.all)
        {
            teamsDic[team] = container.Create<TeamBuildingsSystem>(new(){team});
        }
    }

    public void Update()
    {
        foreach (var (k, v) in teamsDic)
        {
            v.Update();
        }
    }

    public void Add(Building b)
    {
        all.Add(b);
        ForTeam(b.teamComp.team).all.Add(b);
    }

    public void Remove(Building b)
    {
        all.Remove(b);
        ForTeam(b.teamComp.team).all.Remove(b);
    }

    public TeamBuildingsSystem ForTeam(Team team)
    {
        return teamsDic[team];
    }
}

public class TeamBuildingsSystem
{
    public Team team;

    public List<Building> all = new();

    public TeamBuildingsSystem(Team team)
    {
        this.team = team;
    }

    public void Init()
    {
                
    }

    public void Update()
    {
        all.RemoveAll(b => b == null);        
    }
}