using System;
using UnityEngine;

// public class CastleTileAppearAnim : CastleTileAnim
// {
//     public float progress;
//     public MaterialPropertyBlock materialPropertyBlock;

//     public CastleTileAppearAnim(Tile target) : base(target)
//     {
//         materialPropertyBlock = new();

//         target.floorSpriteRenderer.GetPropertyBlock(materialPropertyBlock);

//         materialPropertyBlock.SetVector("_offset", new Vector2(UnityEngine.Random.Range(0, 1000), UnityEngine.Random.Range(0, 1000)));
//         materialPropertyBlock.SetFloat("_density", target.cmsEnt.Get<CmsShaderDensityComp>().density);

//         target.floorSpriteRenderer.SetPropertyBlock(materialPropertyBlock);
//     }

//     public override void Process()
//     {
//         target.floorSpriteRenderer.GetPropertyBlock(materialPropertyBlock);

//         materialPropertyBlock.SetFloat("_time", progress);

//         target.floorSpriteRenderer.SetPropertyBlock(materialPropertyBlock);
//     }

//     public override bool ShouldComplete() => progress > 1.0f;
// }

// public class CastleTileDisappearAnim : CastleTileAnim
// {
//     public float progress;
//     public MaterialPropertyBlock materialPropertyBlock;
    
//     public CastleTileDisappearAnim(Tile target) : base(target)
//     {
//         materialPropertyBlock = new();

//         target.floorSpriteRenderer.GetPropertyBlock(materialPropertyBlock);

//         materialPropertyBlock.SetVector("_offset", new Vector2(UnityEngine.Random.Range(0, 1000), UnityEngine.Random.Range(0, 1000)));
//         materialPropertyBlock.SetFloat("_density", target.cmsEnt.Get<CmsShaderDensityComp>().density);

//         target.floorSpriteRenderer.SetPropertyBlock(materialPropertyBlock);
//     }

//     public override void Process()
//     {
//         target.floorSpriteRenderer.GetPropertyBlock(materialPropertyBlock);

//         materialPropertyBlock.SetFloat("_time", progress);

//         target.floorSpriteRenderer.SetPropertyBlock(materialPropertyBlock);
//     }

//     public override bool ShouldComplete() => progress > 1.0f;
// }

// public class CastleTileAnim
// {
//     public Tile target;

//     public CastleTileAnim(Tile target)
//     {
//         this.target = target;    
//     }

//     public virtual void Process()
//     {
//     }

//     public virtual void Complete()
//     {
                
//     }
//     public virtual bool ShouldComplete()
//     {
//         return true;
//     }
// }

// [Serializable]
// public class CmsCastleTile
// {
    
// }

public class CastleTile : Tile
{
    [NonSerialized] public Building build;
    [NonSerialized] public float appearProgress = 1.0f;
    [NonSerialized] public MaterialPropertyBlock materialPropertyBlock;
    
    public override void Init()
    {
        materialPropertyBlock = new();
        floorSpriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetVector("_offset", new Vector2(UnityEngine.Random.Range(0, 1000), UnityEngine.Random.Range(0, 1000)));
        materialPropertyBlock.SetFloat("_density", cmsEnt.Get<CmsShaderDensityComp>().density);
        floorSpriteRenderer.SetPropertyBlock(materialPropertyBlock);
    
        base.Init();
    }
    public override void Update()
    {
        floorSpriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat("_time", appearProgress);
        floorSpriteRenderer.SetPropertyBlock(materialPropertyBlock);
    
        base.Update();
    }
}