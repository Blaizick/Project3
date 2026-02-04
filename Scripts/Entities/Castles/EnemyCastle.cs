using System;
using System.Collections.Generic;
using System.Linq;
using BJect;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCastle : Castle
{
    [Inject] public Player player;

    public override void Update()
    {
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
            SetMovePos(target.transform.position);
        }

        base.Update();
    }
}