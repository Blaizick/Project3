using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJect;
using UnityEngine;

public static class ProjectileUtils
{
    public static string GetProjDesc(CmsEnt proj)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Name: {proj.Get<CmsNameComp>().name}");
        sb.AppendLine($"Description: {proj.Get<CmsDescComp>().desc}");
        sb.AppendLine($"Count: {proj.Get<CmsAmmoResourceComp>().ammoCount} for 1 {proj.Get<CmsAmmoResourceComp>().resource.Get<CmsNameComp>().name}");
        sb.AppendLine($"Damage: {proj.Get<CmsDamageComp>().damage}");
        return sb.ToString();
    }

    /// <summary>
    /// Sorts projectiles descendent by damage
    /// </summary>
    /// <param name="projectiles"></param>
    /// <returns></returns>
    public static List<CmsEnt> SortProjectiles(List<CmsEnt> projectiles)
    {
        return projectiles.OrderBy(i => -i.Get<CmsDamageComp>().damage).ToList();
    }
}

public class Projectile : MonoBehaviour
{
    [NonSerialized] public CmsEnt cmsEnt;
    public Rigidbody2D rb;
    public TeamComp teamComp;

    [NonSerialized] public Vector2 targetPos;

    public virtual void Init()
    {
        Destroy(gameObject, cmsEnt.Get<CmsLifetimeComp>().lifetime);
    }

    public virtual void Update()
    {
    }

    public class Factory : BaseFactory
    {
        public Factory(DiContainer container) : base(container) {}

        public Projectile Use(CmsEnt cmsEnt, Team team, Vector2 origin, Vector2 target, Quaternion rot)
        {
            var scr = container.Instantiate(cmsEnt.Get<CmsProjectilePfbComp>().pfb);
            scr.transform.position = origin;
            scr.transform.rotation = rot;
            scr.cmsEnt = cmsEnt;
            scr.teamComp.team = team;
            scr.targetPos = target;
            scr.Init();
            return scr;
        }
    }
}

[Serializable]
public class CmsMoveSpeedComp : CmsComp
{
    public float moveSpeed;
}

[Serializable]
public class CmsDamageComp : CmsComp
{
    public float damage;
}

[Serializable]
public class CmsProjectilePfbComp : CmsComp
{
    public Projectile pfb;
}