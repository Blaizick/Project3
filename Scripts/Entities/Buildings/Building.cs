using System;
using BJect;
using UnityEngine;

[Serializable]
public class CmsResourceRequirementComp : CmsResourceStackComp
{
    
}

public class Building : MonoBehaviour
{
    [NonSerialized] public CmsEnt cmsEnt;

    [NonSerialized] public Vector2Int pos;
    [NonSerialized] public CastleGrid grid;

    public HealthComp healthComp;
    public TeamComp teamComp;

    public virtual void Init()
    {
        healthComp.Set(cmsEnt);
        healthComp.Init();
    }

    public virtual void Update()
    {
        healthComp.Update();
    }

    public class Factory : BaseFactory
    {
        public Factory(Container container) : base(container) {}

        public Building Use(CmsEnt cmsEnt, Team team, Transform root)
        {
            var scr = container.Instantiate<Building>(cmsEnt.Get<CmsPfbComp>().pfb, root);
            scr.teamComp.Set(team);
            scr.cmsEnt = cmsEnt;
            scr.Init();
            return scr;
        }
    }
}