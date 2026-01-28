using System;
using System.Collections.Generic;
using System.Linq;
using BJect;
using Mono.Cecil;
using UnityEngine;

public class CastleGrid : MonoBehaviour
{
    [NonSerialized] public List<Tile> tiles = new();
    [NonSerialized] public int size;

    [Inject, NonSerialized] public Building.Factory buildFactory;
    [NonSerialized] public Castle castle;

    [Inject, NonSerialized] public ResourcesSystem resources;

    [Inject, NonSerialized] public Tile.Factory tileFactory;

    public void Init()
    {
        
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
        // Debug.Log($"gPos: {gPos}, worldPos: {GridToWorldPos(gPos)}, " +
        //     $"centerPos: {GeometryUtils.BuildAnchorToWorldPos(GridToWorldPos(gPos), bSize)}");
        return GeometryUtils.BuildAnchorToWorldPos(GridToWorldPos(gPos), bSize);
    }

    public Tile GetTileAt(Vector2 pos)
    {
        int id = GridPosToI(WorldToGridPos(pos));
        return id >= tiles.Count || id < 0 ? null : tiles[id];
    }

    public bool IsBuildInBounds(Vector2Int pos, int size)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x + size <= this.size && pos.y + size <= this.size;
    }

    public bool IntersectsWithBuild(Vector2Int pos, int size)
    {
        for (int i = 0; i < size * size; i++)
        {
            int x = pos.x + i % size;
            int y = pos.y + i / size;

            if (tiles[x + y * this.size].build != null)
            {
                return true;
            }
        }
        return false;
    }

    public bool CanPlace(Vector2Int pos, int size)
    {
        return IsBuildInBounds(pos, size) && !IntersectsWithBuild(pos, size);
    }

    public bool HasAllResourcesForBlock(CmsEnt cmsEnt)
    {
        return resources.Has(ResourcesUtils.GetAllStacks<CmsResourceRequirementComp>(cmsEnt));
    }

    public Building PlaceBlock(CmsEnt cmsEnt, Vector2Int pos)
    {
        if (cmsEnt == null)
            return null;

        var size = cmsEnt.Get<CmsGridSizeComp>();

        var b = buildFactory.Use(cmsEnt, castle.teamComp.team, transform);
        b.pos = pos;

        for (int i = 0; i < size.size * size.size; i++)
        {
            int x = i % size.size + pos.x;
            int y = i / size.size + pos.y;
            
            tiles[x + y * this.size].build = b;
        }

        b.transform.position = GetBuildPosition(pos, size.size);

        resources.Remove(ResourcesUtils.GetAllStacks<CmsResourceRequirementComp>(cmsEnt));

        b.Init();
        return b;
    }

    public void Set(CmsEnt cmsEnt, Castle castle)
    {
        this.castle = castle;
        size = cmsEnt.Get<CmsGridSizeComp>().size;
        
        var floorPfb = cmsEnt.Get<CmsFloorPfbComp>();

        for (int i = 0; i < size * size; i++)
        {
            int x = i % size;
            int y = i / size;

            var pfb = i % 2 == 0 ? floorPfb.floorA : floorPfb.floorB;

            var s = tileFactory.Use(pfb, transform, new(x, y), this);
            s.transform.position = GridToWorldPos(new Vector2Int(x, y));
                
            tiles.Add(s);
        }
    }
}

public static class GeometryUtils
{
    public static Vector2 BuildAnchorToWorldPos(Vector2 pos, int size)
    {
        float hs = (size - 1) * 0.5f;
        return pos + new Vector2(hs, hs);
    }

    public static Vector2Int GetBuildAnchor(Vector2Int pos, int size)
    {
        int offset = (size + 1) % 2;
        return pos - new Vector2Int(size, size) / 2 + new Vector2Int(offset, offset);
    }
}