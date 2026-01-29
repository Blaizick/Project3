using System;
using BJect;
using UnityEngine;

public class Turret : Building
{
    public GameObject towerRoot;

    [NonSerialized] public CmsAttackRangeComp attackRangeComp;
    [NonSerialized] public CmsRotationComp rotationComp;
    [NonSerialized] public CmsProjectileComp projectileComp;

    [Inject] public Projectile.Factory projectileFactory;

    public ReloadComp reloadComp;

    [NonSerialized] public GameObject target;

    public override void Init()
    {
        reloadComp = new(cmsEnt.Get<CmsReloadTimeComp>().reloadTime);

        rotationComp = cmsEnt.Get<CmsRotationComp>();
        attackRangeComp = cmsEnt.Get<CmsAttackRangeComp>();
        projectileComp = cmsEnt.Get<CmsProjectileComp>();

        base.Init();
    }

    public override void Update()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, attackRangeComp.attackRange);
        target = null;
        float prevDst = 0;
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IHealthComp>(out var h) && 
                hit.TryGetComponent<TeamComp>(out var t) && 
                t.team.IsEnemy(teamComp.team) &&
                h.CanDamage)
            {
                float dst = Vector2.Distance(transform.position, hit.transform.position);
                if (target == null || dst < prevDst)
                {
                    prevDst = dst;
                    target = hit.gameObject;
                }
            }
        }

        if (target != null)
        {
            Quaternion targetRot = MathUtils.GetLookAtRotation(transform.position, target.transform.position);
            targetRot = MathUtils.OffsetRotation(targetRot, rotationComp.offset);
            towerRoot.transform.rotation = Quaternion.RotateTowards(towerRoot.transform.rotation, targetRot, Time.deltaTime * rotationComp.rotationSpeed);
        
            if (reloadComp.Reloaded)
            {
                projectileFactory.Use(projectileComp.proj, teamComp.team, transform.position, target.transform.position);
                reloadComp.Reset();
            }
        }
        
        reloadComp.Update();

        base.Update();
    }
}

public class ReloadComp
{
    public float progress;
    public float reloadTime;

    public bool Reloaded => progress > 1.0f;

    public ReloadComp(float reloadTime)
    {
        this.reloadTime = reloadTime;
    }

    public void Update()
    {
        progress += Time.deltaTime / reloadTime;
    }
    public void Reset()
    {
        progress = 0.0f;
    }
}

[Serializable]
public class CmsAttackRangeComp : CmsComp
{
    public float attackRange;
}

[Serializable]
public class CmsRotationComp : CmsComp
{
    public float offset;
    public float rotationSpeed;
}

[Serializable]
public class CmsReloadTimeComp : CmsComp
{
    public float reloadTime;
}

[Serializable]
public class CmsProjectileComp : CmsComp
{
    public CmsEnt proj;
}