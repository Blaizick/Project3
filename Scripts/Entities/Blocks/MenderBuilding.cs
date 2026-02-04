
using System;
using UnityEngine;

public class MenderBuilding : Building
{
    public override void Init()
    {
        base.Init();
    }

    public override void Update()
    {
        float heal = Time.deltaTime * cmsEnt.Get<CmsHealComp>().heal;

        var hits = Physics2D.OverlapCircleAll(transform.position, cmsEnt.Get<CmsRangeComp>().range);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IHealthComp>(out var h) &&
                hit.TryGetComponent<TeamComp>(out var t) &&
                t.team.IsAlly(teamComp.team))
            {
                h.Heal(heal);
            }
        }

        busy = true;

        base.Update();
    }
}

[Serializable]
public class CmsRangeComp : CmsComp
{
    public float range;
}

[Serializable]
public class CmsHealComp : CmsComp
{
    public float heal;
}