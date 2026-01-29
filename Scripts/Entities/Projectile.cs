using System;
using BJect;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public CmsEnt cmsEnt;
    public Rigidbody2D rb;
    public TeamComp teamComp;

    public Vector2 target;

    public CmsMoveSpeedComp moveSpeedComp;

    public void Init()
    {
        moveSpeedComp = cmsEnt.Get<CmsMoveSpeedComp>();
    }

    public void Update()
    {
        if (MathUtils.IsWithin(transform.position, target))
        {
            Destroy(gameObject);
        }
        rb.linearVelocity = (target - (Vector2)transform.position).normalized * moveSpeedComp.moveSpeed;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IHealthComp>(out var h) && 
            collision.TryGetComponent<TeamComp>(out var t) && 
            t.team.IsEnemy(teamComp.team) &&
            h.CanDamage)
        {
            h.TakeDamage(cmsEnt.Get<CmsDamageComp>().damage);
            Destroy(gameObject);
        }
    }

    public class Factory : BaseFactory
    {
        public Factory(Container container) : base(container) {}

        public Projectile Use(CmsEnt cmsEnt, Team team, Vector2 origin, Vector2 target)
        {
            var scr = container.Instantiate(cmsEnt.Get<CmsProjectilePfbComp>().pfb);
            scr.transform.position = origin;
            scr.cmsEnt = cmsEnt;
            scr.teamComp.team = team;
            scr.target = target;
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