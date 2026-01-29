using System;
using System.Text;
using BJect;
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

    public int Size => cmsEnt.Get<CmsGridSizeComp>().size;

    public virtual void Init()
    {
        healthComp.Set(cmsEnt);
        healthComp.Init();
        healthComp.onDie.AddListener(() =>
        {
            grid.BreakBuild(this);
        });
    }

    public virtual void Update()
    {
        healthComp.canDamage = true;
        healthComp.Update();
    }

    public virtual bool CanBreak()
    {
        return true;
    }
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
}

[Serializable]
public class CmsDescComp : CmsComp
{
    public string desc;
}