using System;
using BJect;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [NonSerialized] public BGrid grid;
    [NonSerialized] public Vector2Int pos;
    [NonSerialized] public CmsEnt cmsEnt;
    
    public virtual void Init()
    {
        
    }
    public virtual void Update()
    {
    }

    public class Factory : BaseFactory
    {
        public Factory(DiContainer container) : base(container)
        {
        }

        public Tile Use(CmsEnt cmsEnt, Transform tr, Vector2Int pos, BGrid grid)
        {
            var scr = container.Instantiate(cmsEnt.Get<CmsTilePfbComp>().pfb, tr);
            scr.cmsEnt = cmsEnt;
            scr.pos = pos;
            scr.grid = grid;
            scr.Init();
            return scr;
        }
    }
}

[Serializable]
public class CmsTilePfbComp : CmsComp
{
    public Tile pfb;
}

[Serializable]
public class CmsGridSizeComp : CmsComp
{
    public int size;
}
[Serializable]
public class CmsOffsetComp : CmsComp
{
    public Vector2 offset;
}

[Serializable]
public class CmsFloorVariationsComp : CmsComp
{
    public CmsEnt a;
    public CmsEnt b;
}

[Serializable]
public class CmsAppearTimeComp : CmsComp
{
    public float appearTime;
}