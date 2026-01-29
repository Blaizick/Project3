using System;
using BJect;
using JetBrains.Annotations;
using UnityEngine;

public class Castle : MonoBehaviour
{
    [NonSerialized] public CmsEnt cmsEnt;

    public BoxCollider2D col;
    public Rigidbody2D rb;

    public CastleGrid grid;

    public TeamComp teamComp;
    public HealthComp healthComp;

    public virtual void Init()
    {
        healthComp.Set(cmsEnt);
        healthComp.Init();
    
        healthComp.onDie.AddListener(() => Destroy(gameObject));
    }

    public virtual void Update()
    {
        healthComp.canDamage = !grid.HasBuilding();
    }

    public class Factory : BaseFactory
    {
        public Tile.Factory tileFactory;

        public Factory(Container container, Tile.Factory tileFactory) : base(container)
        {
            this.tileFactory = tileFactory;
        }

        public Castle Use(CmsEnt cmsEnt, Team team)
        {
            var scr = container.Instantiate(cmsEnt.Get<CmsCastlePfbComp>().pfb);
            scr.cmsEnt = cmsEnt;
            scr.teamComp.team = team;
            
            scr.grid.Set(cmsEnt, scr);
            
            Vector2 sizeV = new(scr.grid.size, scr.grid.size);
            scr.col.size = sizeV;
            scr.col.offset = Vector2.zero;
            
            scr.Init();
            scr.grid.Init();

            return scr;
        }
    }
}

[Serializable]
public class CmsCastlePfbComp : CmsComp
{
    public Castle pfb;
}