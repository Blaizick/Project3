using System;
using System.Linq;
using BJect;
using UnityEngine;

public class EnemyCastle : Castle
{
    [Inject] public Player player;

    public override void Update()
    {
        moving = false;

        var hits = Physics2D.OverlapCircleAll(transform.position, cmsEnt.Get<CmsAgreRangeComp>().range).ToList();
        Castle target = null;
        float minDst = 0.0f;
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Castle>(out var c) && 
                c.teamComp.team.IsEnemy(teamComp.team))
            {
                float dst = Vector2.Distance(transform.position, hit.transform.position);
                if (!target || dst < minDst)
                {
                    target = c;
                    minDst = dst;
                }
            }
        }
        if (target)
        {
            if (!Physics2D.RaycastAll(transform.position, 
                    target.transform.position - transform.position, 
                    cmsEnt.Get<CmsTargetMinDstComp>().minDst).
                ToList().
                Find(r => r.transform.gameObject == target.gameObject))
            {
                SetMovePos(target.transform.position);
            }
        }

        base.Update();
    }
}

[Serializable]
public class CmsTargetMinDstComp : CmsComp
{
    public float minDst;
}