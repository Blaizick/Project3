using System;
using System.Collections.Generic;
using BJect;
using UnityEngine;

public class BGrid : MonoBehaviour
{
    public List<Tile> Tiles {get;set;}= new();
    [NonSerialized] public int size;

    [Inject, NonSerialized] public CastleTile.Factory tileFactory;

    [NonSerialized] public CmsEnt cmsEnt;

    [Inject, NonSerialized] public DiContainer container;

    public void Init()
    {
        
    }

    public virtual void Update()
    {
        
    }

    public virtual void Set(CmsEnt cmsEnt)
    {
        this.cmsEnt = cmsEnt;
        size = cmsEnt.Get<CmsGridSizeComp>().size;
        
        var variations = cmsEnt.Get<CmsFloorVariationsComp>();

        for (int i = 0; i < size * size; i++)
        {
            int x = i % size;
            int y = i / size;

            int tmp = x + y * size + ((size % 2 == 0) ? y % 2 : 0);
            var variation = tmp % 2 == 0 ? variations.a : variations.b;

            var s = tileFactory.Use(variation, transform, new(x, y), this);
            s.transform.position = GridToWorldPos(new Vector2Int(x, y));
                
            Tiles.Add(s);
        }
    }

    public int GridPosToI(Vector2Int vec)
    {
        return vec.x + vec.y * size;
    }

    public Vector2 LocalOffset => new Vector2(size - 1, size - 1) * -0.5f;

    public Vector2 GridToWorldPos(Vector2Int pos)
    {
        return (Vector2)transform.position + pos + LocalOffset; 
    }
    public Vector2Int WorldToGridPos(Vector2 pos)
    {
        Vector2 tmp = pos - (Vector2)transform.position + Vector2.one * 0.5f - LocalOffset;
        return new Vector2Int((int)tmp.x, (int)tmp.y);
    }
    public Vector2 GetBuildPosition(Vector2Int gPos, int bSize)
    {
        return GeometryUtils.BuildAnchorToWorldPos(GridToWorldPos(gPos), bSize);
    }
    public CastleTile GetTileAt(Vector2 pos)
    {
        int id = GridPosToI(WorldToGridPos(pos));
        return id >= Tiles.Count || id < 0 ? null : (CastleTile)Tiles[id];
    }
}