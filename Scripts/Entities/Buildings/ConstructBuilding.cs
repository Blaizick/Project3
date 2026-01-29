using System;
using BJect;
using UnityEngine;

[Serializable]
public class CmsConstructTimeComp : CmsComp
{
    public float time;
}

public class ConstructBuilding : Building
{
    [Inject, NonSerialized] public ResourcesSystem resources;

    [NonSerialized] public float progress;
    
    public SpriteRenderer spriteRenderer;

    public override void Init()
    {
        spriteRenderer.sprite = cmsEnt.Get<CmsSpriteComp>().sprite;

        base.Init();
    }

    public override void Update()
    {
        progress += Time.deltaTime / cmsEnt.Get<CmsConstructTimeComp>().time;
        if (progress > 1.0f)
        {
            Destroy(gameObject);
            grid.PlaceBlock(cmsEnt, pos);
        }

        base.Update();        
    }
}