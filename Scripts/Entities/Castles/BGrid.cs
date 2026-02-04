using System;
using System.Collections.Generic;
using BJect;
using UnityEngine;

public class BGrid : MonoBehaviour
{
    [NonSerialized] public List<Tile> tiles = new();
    [NonSerialized] public int size;

    [Inject, NonSerialized] public Tile.Factory tileFactory;

    [NonSerialized] public CmsEnt cmsEnt;

    [Inject, NonSerialized] public DiContainer container;

    public void Init()
    {
        
    }

    public virtual void Set(CmsEnt cmsEnt)
    {
        size = cmsEnt.Get<CmsGridSizeComp>().size;
        
        var floorComp = cmsEnt.Get<CmsFloorPfbComp>();

        for (int i = 0; i < size * size; i++)
        {
            int x = i % size;
            int y = i / size;

            var pfb = i % 2 == 0 ? floorComp.floorA : floorComp.floorB;

            var s = tileFactory.Use(pfb, transform, new(x, y), this);
            s.transform.position = GridToWorldPos(new Vector2Int(x, y));
                
            tiles.Add(s);
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
    public Tile GetTileAt(Vector2 pos)
    {
        int id = GridPosToI(WorldToGridPos(pos));
        return id >= tiles.Count || id < 0 ? null : tiles[id];
    }
}