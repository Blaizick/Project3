using System;
using BJect;
using DG.Tweening;
using UnityEngine;

public class ForceField : Building
{
    public GameObject forceFieldRoot;
    public Collider2D forceFieldCol;

    [NonSerialized] public bool active = true;
    public HealthComp forceFieldHealthComp;
    public TeamComp forceFieldTeamComp;

    [NonSerialized] public Vector2 forceFieldSize;

    [NonSerialized] public ReloadComp reloadComp;

    [Inject, NonSerialized] public DiContainer container;

    [NonSerialized] public Tween scaleTween;

    public override void Init()
    {
        forceFieldHealthComp.Set(cmsEnt.Get<CmsForceFieldHealthComp>().health);
        forceFieldHealthComp.Init();
        forceFieldHealthComp.onDie.AddListener(() =>
        {
            if (scaleTween != null)
            {
                scaleTween.Complete();
            }
            scaleTween = forceFieldRoot.transform.DOScale(new Vector3(0, 0, 1), 
                cmsEnt.Get<CmsForceFieldAppearAnimationDurationComp>().duration).
                OnComplete(() =>
                {
                    forceFieldRoot.SetActive(false);                
                });
            active = false;
        });

        forceFieldTeamComp.Set(teamComp.team);

        forceFieldSize = forceFieldRoot.transform.localScale;

        reloadComp = new(cmsEnt.Get<CmsReloadTimeComp>().reloadTime);

        base.Init();
    }

    public override void Update()
    {
        if (!active && reloadComp.Reloaded)
        {
            if (scaleTween != null)
            {
                scaleTween.Complete();
            }
            reloadComp.Reset();
            forceFieldRoot.SetActive(true);
            forceFieldRoot.transform.localScale = new(0, 0, 1);
            scaleTween = forceFieldRoot.transform.DOScale(new Vector3(forceFieldSize.x, forceFieldSize.y, 1), 
                cmsEnt.Get<CmsForceFieldAppearAnimationDurationComp>().duration);
            forceFieldHealthComp.health = cmsEnt.Get<CmsForceFieldHealthComp>().health;
            active = true;
        }
        
        forceFieldCol.enabled = active;
        busy = active;

        reloadComp.Update();

        base.Update();
    }

    public override void OnDestroy()
    {
        forceFieldRoot.transform.DOKill();
        base.OnDestroy();
    }
}

[Serializable]
public class CmsForceFieldHealthComp : CmsComp
{
    public float health;
}

[Serializable]
public class CmsForceFieldAppearAnimationDurationComp : CmsComp
{
    public float duration;
}