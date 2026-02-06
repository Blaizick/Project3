using System;
using BJect;
using UnityEngine;

public class CastleGrid : BGrid
{
    [NonSerialized] public Castle castle;

    [Inject, NonSerialized] public ResourcesSystem resources;
    [Inject, NonSerialized] public BuildingsSystem buildings;

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

            if (((CastleTile)Tiles[GridPosToI(new(x, y))]).build != null)
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
        if (resources.ForTeam(castle.teamComp.team).InfinityResources)
        {
            return true;
        }
        return resources.ForTeam(castle.teamComp.team).Has(ResourcesUtils.GetAllStacks<CmsResourceRequirementComp>(cmsEnt));
    }

    public Building StartConstructing(CmsEnt cmsEnt, Vector2Int pos)
    {
        if (!resources.ForTeam(castle.teamComp.team).InfinityResources)
        {
            resources.ForTeam(castle.teamComp.team).Remove(ResourcesUtils.GetAllStacks<CmsResourceRequirementComp>(cmsEnt));
        }
        return PlaceBlock(cmsEnt, Blocks.constructBuilding.Get<CmsPfbComp>().pfb.GetComponent<Building>(), pos);
    }

    public Building PlaceBlock(CmsEnt cmsEnt, Vector2Int pos)
    {
        return PlaceBlock(cmsEnt, cmsEnt.Get<CmsPfbComp>().pfb.GetComponent<Building>(), pos);
    }
    public Building PlaceBlock(CmsEnt cmsEnt, Building pfb, Vector2Int pos)
    {
        if (cmsEnt == null)
            return null;

        var size = cmsEnt.Get<CmsGridSizeComp>();

        var b = container.Instantiate(pfb, transform);
        b.cmsEnt = cmsEnt;
        b.grid = this;
        b.pos = pos;
        b.teamComp.team = castle.teamComp.team;

        for (int i = 0; i < size.size * size.size; i++)
        {
            int x = i % size.size + pos.x;
            int y = i / size.size + pos.y;
            
            ((CastleTile)Tiles[GridPosToI(new(x, y))]).build = b;
        }

        buildings.Add(b);

        b.transform.position = GetBuildPosition(pos, size.size);

        b.Init();
        return b;
    }

    public void Set(CmsEnt cmsEnt, Castle castle)
    {
        this.castle = castle;
        Set(cmsEnt);
    }

    public void BreakBuild(Building build)
    {
        for (int i = 0; i < build.Size * build.Size; i++)
        {
            int x = build.pos.x + i % build.Size;
            int y = build.pos.y + i / build.Size;
        
            ((CastleTile)Tiles[GridPosToI(new(x, y))]).build = null;
        }
        buildings.Remove(build);
        Destroy(build.gameObject);
    }

    public bool HasBuilding()
    {
        return Tiles.FindIndex(t => ((CastleTile)t).build != null) > -1;
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