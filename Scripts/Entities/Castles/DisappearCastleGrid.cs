using System;
using BJect;
using UnityEngine;

public class CastleDisappearSystem
{
    public DiContainer container;

    public CastleDisappearSystem(DiContainer container)
    {
        this.container = container;
    }

    public void Disappear(Castle castle)
    {
        var scr = container.Instantiate(Profiles.basePrefabs.Get<CmsDisappearCastleGridPfbComp>().pfb);
        scr.transform.position = castle.transform.position;
        scr.Set(castle.cmsEnt);
        scr.Init();
    }
}

public class DisappearCastleGrid : BGrid
{
    [NonSerialized] public float disappearProgress;

    public override void Update()
    {
        disappearProgress += Time.deltaTime / cmsEnt.Get<CmsDisappearTimeComp>().disappearTime;
        foreach (var t in Tiles)
        {
            ((CastleTile)t).appearProgress = 1.0f - disappearProgress;
        }

        if (disappearProgress > 1.0f)
        {
            Destroy(gameObject);
        }        
    
        base.Update();
    }
}

[Serializable]
public class CmsDisappearTimeComp : CmsComp
{
    public float disappearTime;
}

[Serializable]
public class CmsDisappearCastleGridPfbComp : CmsComp
{
    public DisappearCastleGrid pfb;
}