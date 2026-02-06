using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJect;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class CmsRecoilComp : CmsComp
{
    public float streangth;
    public float duration;
    public int vibrato = 10;
    public float elasticity = 1;
    public bool snapping = false;
}

[Serializable]
public class CmsMaxAmmoStack
{
    public float max;
}

public class Turret : Building
{
    public GameObject towerRoot;

    [NonSerialized] public CmsAttackRangeComp attackRangeComp;
    [NonSerialized] public CmsRotationComp rotationComp;
    [NonSerialized] public CmsProjectileComp projectileComp;

    [Inject] public Projectile.Factory projectileFactory;

    public ReloadComp reloadComp;

    [NonSerialized] public GameObject target;
    [NonSerialized, Inject] public ResourcesSystem resources; 

    [NonSerialized] public CmsEnt ammo;
    [NonSerialized] public float ammoCount;
    [NonSerialized] public List<CmsEnt> projectilesList;
    [NonSerialized] public float findTargetProgress;

    public override void Init()
    {
        reloadComp = new(cmsEnt.Get<CmsReloadTimeComp>().reloadTime);

        rotationComp = cmsEnt.Get<CmsRotationComp>();
        attackRangeComp = cmsEnt.Get<CmsAttackRangeComp>();
        projectileComp = cmsEnt.Get<CmsProjectileComp>();
        projectilesList = new();
        foreach (var p in cmsEnt.GetAll<CmsProjectileComp>())
        {
            projectilesList.Add(p.proj);
        }
        projectilesList = ProjectileUtils.SortProjectiles(projectilesList);

        base.Init();
    }

    public override void Update()
    {
        findTargetProgress += Time.deltaTime;
        
        if (findTargetProgress >= 1.0f)
        {
            target = null;

            var hits = Physics2D.OverlapCircleAll(transform.position, attackRangeComp.attackRange);
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

            findTargetProgress = 0.0f;
        }
        
        busy = target;

        if (target != null)
        {
            Quaternion targetRot = MathUtils.GetLookAtRotation(transform.position, target.transform.position);
            targetRot = MathUtils.RotateRotation(targetRot, rotationComp.offset);
            towerRoot.transform.rotation = Quaternion.RotateTowards(towerRoot.transform.rotation, targetRot, Time.deltaTime * rotationComp.rotationSpeed);
        
            if (reloadComp.Reloaded)
            {
                if (ammoCount < 1f)
                {
                    ammo = null;

                    if (resources.ForTeam(teamComp.team).InfinityResources)
                    {
                        ammo = projectilesList.First();
                        ammoCount = projectilesList.First().Get<CmsAmmoResourceComp>().ammoCount;    
                    }
                    else
                    {
                        ammo = null;
                        foreach (var p in projectilesList)
                        {
                            var ammoResource = p.Get<CmsAmmoResourceComp>();
                            if (resources.ForTeam(teamComp.team).Has(new ResourceStack(ammoResource.resource, 1)))
                            {
                                ammo = p;
                                break;
                            }
                        }
                        if (ammo)
                        {
                            var ammoResourceComp = ammo.Get<CmsAmmoResourceComp>();
                            resources.ForTeam(teamComp.team).Remove(ammoResourceComp.resource, 1.0f);
                            ammoCount = ammoResourceComp.ammoCount;
                        }
                    }
                }
                if (ammoCount >= 1f)
                {
                    Quaternion rot = Quaternion.Euler(0, 0, towerRoot.transform.rotation.eulerAngles.z - rotationComp.offset);

                    projectileFactory.Use(projectileComp.proj, 
                        teamComp.team, 
                        transform.position, 
                        target.transform.position, 
                        rot);
                    reloadComp.Reset();
          
                    var recoilComp = cmsEnt.Get<CmsRecoilComp>();
                    towerRoot.transform.DOPunchPosition(MathUtils.RotationToDirection(MathUtils.RotateRotation(rot, 180)) * recoilComp.streangth, 
                        recoilComp.duration,
                        recoilComp.vibrato,
                        recoilComp.elasticity,
                        recoilComp.snapping);    
                
                    ammoCount--;
                }
            }
        }
        
        reloadComp.Update();

        base.Update();
    }

    public override string Desc
    {
        get
        {
            StringBuilder sb = new(base.Desc);
            sb.AppendLine("Awailable projectiles: ");
            foreach (var i in cmsEnt.GetAll<CmsProjectileComp>())
            {
                sb.AppendLine(ProjectileUtils.GetProjDesc(i.proj));
            }
            if (ammo)
            {
                sb.AppendLine("Current projectile: ");
                sb.AppendLine(ProjectileUtils.GetProjDesc(ammo));
                sb.AppendLine($"Count: {(int)ammoCount}");
            }
            return sb.ToString();
        }
    }

    public override void OnDestroy()
    {
        towerRoot.transform.DOKill();
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

[Serializable]
public class CmsAmmoResourceComp : CmsComp
{
    public CmsEnt resource;
    public float ammoCount;
}