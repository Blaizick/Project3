using System;
using System.Collections.Generic;
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
    [NonSerialized] public float appearProgress = 1.0f;

    [NonSerialized] public  MaterialPropertyBlock propBlock;

    public override void Init()
    {
        propBlock = new();
        foreach (var t in Tiles)
        {
            ((CastleTile)t).baseState.root.SetActive(false);
            ((CastleTile)t).constructState.root.SetActive(true);
            ((CastleTile)t).constructState.spriteRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat("_density", t.cmsEnt.Get<CmsShaderDensityComp>().density);
            propBlock.SetVector("_offset", 
                new Vector2(UnityEngine.Random.Range(-1000, 1000), 
                UnityEngine.Random.Range(-1000, 1000)));
            ((CastleTile)t).constructState.spriteRenderer.SetPropertyBlock(propBlock);
        }
        
        base.Init();
    }

    public override void Update()
    {
        appearProgress -= Time.deltaTime * cmsEnt.Get<CmsDisappearTimeComp>().disappearTime;
        if (appearProgress <= 0)
        {
            Destroy(gameObject);
        }
        foreach (var t in Tiles)
        {
            ((CastleTile)t).constructState.spriteRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat("_time", appearProgress);
            ((CastleTile)t).constructState.spriteRenderer.SetPropertyBlock(propBlock);
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