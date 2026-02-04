using System;
using BJect;
using UnityEngine;

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