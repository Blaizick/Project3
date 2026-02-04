using System;
using BJect;
using UnityEditor.Callbacks;
using UnityEngine;

[Serializable]
public class CmsConstructTimeComp : CmsComp
{
    public float time;
}

[Serializable]
public class CmsShaderDensityComp : CmsComp
{
    public float density;
}

public class ConstructBuilding : Building
{
    [Inject, NonSerialized] public ResourcesSystem resources;

    [NonSerialized] public float progress;
    
    public SpriteRenderer spriteRenderer;

    [NonSerialized] public MaterialPropertyBlock materialPropertyBlock;

    [NonSerialized] public CmsEnt profile;

    public override void Init()
    {
        col.size = new(Size, Size);

        spriteRenderer.sprite = cmsEnt.Get<CmsSpriteComp>().sprite;

        profile = Blocks.constructBuilding;

        materialPropertyBlock = new();
        spriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetVector("_offset", new Vector2(UnityEngine.Random.Range(0, 1000), UnityEngine.Random.Range(0, 1000)));
        materialPropertyBlock.SetFloat("_density", profile.Get<CmsShaderDensityComp>().density / Size);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);

        base.Init();
    }

    public override void Update()
    {
        spriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat("_time", progress);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);

        progress += Time.deltaTime / cmsEnt.Get<CmsConstructTimeComp>().time;
        if (progress > 1.0f)
        {
            Destroy(gameObject);
            grid.PlaceBlock(cmsEnt, pos);
        }

        base.Update();        
    }

    public override void Unlock()
    {
    }

    // public override string TooltipTitle => "Construct Block";
    public override string Desc => $"Constructing: {BlockUtils.GetTooltipTitle(cmsEnt)}\nProgress: {progress:F2}\n{AdditionalDescInfo}";
}