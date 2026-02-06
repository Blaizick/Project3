using System;

public class SCastleGrid : BGrid
{
    public bool awailable;

    public override void Init()
    {
        foreach (var t in Tiles)
        {
            ((CastleTile)t).baseState.root.SetActive(true);
            ((CastleTile)t).constructState.root.SetActive(false);
        }
        
        base.Init();
    }

    public override void Update()
    {
        foreach (var t in Tiles)
        {
            ((CastleTile)t).baseState.spriteRenderer.color = awailable ? 
                t.cmsEnt.Get<CmsAwailableColComp>().col : 
                t.cmsEnt.Get<CmsUnawailableColComp>().col;
        }

        base.Update();
    }
}

[Serializable]
public class CmsSCastleGridPfbComp : CmsComp
{
    public SCastleGrid pfb;
}