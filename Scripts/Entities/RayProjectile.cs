

using System;
using UnityEngine;

public class RayProjectile : Projectile
{
    public LineRenderer lineRenderer;

    [NonSerialized] public Vector2 startPos;

    public override void Init()
    {
        startPos = transform.position;
        transform.position = targetPos;
        
        transform.rotation = Quaternion.identity;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new Vector3[]
        {
            startPos - targetPos,
            Vector2.zero,
        });

        var hits = Physics2D.OverlapCircleAll(transform.position, cmsEnt.Get<CmsExplodeRangeComp>().explodeRange);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<HealthComp>(out var h) &&
                hit.TryGetComponent<TeamComp>(out var t) &&
                t.team.IsEnemy(teamComp.team) &&
                h.CanDamage)
            {
                h.TakeDamage(cmsEnt.Get<CmsDamageComp>().damage);
            }
        }

        base.Init();
    }
}

[Serializable]
public class CmsLifetimeComp : CmsComp
{
    public float lifetime; 
}

[Serializable]
public class CmsExplodeRangeComp : CmsComp
{
    public float explodeRange;
}