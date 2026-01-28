using System;
using BJect;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public CastleGrid grid;
    public Vector2Int pos;
    public Building build;

    public class Factory : BaseFactory
    {
        public Factory(Container container) : base(container)
        {
        }

        public Tile Use(Tile a, Transform tr, Vector2Int pos, CastleGrid grid)
        {
            var scr = container.Instantiate(a, tr);
            scr.pos = pos;
            scr.grid = grid;
            return scr;
        }
    }
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
public class CmsFloorPfbComp : CmsComp
{
    public Tile floorA;
    public Tile floorB;
}