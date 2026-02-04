using System;
using UnityEngine;

public class LaserProjectile : Projectile
{
    public LineRenderer lineRenderer;

    public override void Init()
    {
        Vector2 tPos = (Vector2)transform.position + (MathUtils.RotationToDirection(transform.rotation) * cmsEnt.Get<CmsDstComp>().dst);

        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new Vector3[]
        {
            Vector2.zero,
            tPos - (Vector2)transform.position,
        });

        var hits = Physics2D.LinecastAll(transform.position, tPos);
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<HealthComp>(out var h) &&
                hit.collider.TryGetComponent<TeamComp>(out var t) &&
                t.team.IsEnemy(teamComp.team) &&
                h.CanDamage)
            {
                h.TakeDamage(cmsEnt.Get<CmsDamageComp>().damage);
            }
        }

        transform.rotation = Quaternion.identity;

        base.Init();
    }
}

[Serializable]
public class CmsDstComp : CmsComp
{
    public float dst;
}