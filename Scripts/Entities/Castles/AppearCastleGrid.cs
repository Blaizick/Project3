using System;
using System.Collections.Generic;
using BJect;
using UnityEngine;

public class AppearCastleGrid : BGrid
{
    public Team team;

    [Inject, NonSerialized] public CastlesSystem castles;

    [NonSerialized] public float appearProgress;

    [NonSerialized] public MaterialPropertyBlock propBlock;

    public override void Init()
    {
        propBlock = new();

        foreach (var t in Tiles)
        {
            ((CastleTile)t).baseState.root.SetActive(false);
            ((CastleTile)t).constructState.root.SetActive(true);

            propBlock.Clear();
            ((CastleTile)t).constructState.spriteRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat("_density", t.cmsEnt.Get<CmsShaderDensityComp>().density);
            propBlock.SetVector("_offset", 
                new Vector2(UnityEngine.Random.Range(-1000, 1000), 
                UnityEngine.Random.Range(-1000, 1000)));
            ((CastleTile)t).constructState.spriteRenderer.SetPropertyBlock(propBlock);
        }
        
        base.Init();
    }

    public override void Update()
    {
        appearProgress += Time.deltaTime / cmsEnt.Get<CmsAppearTimeComp>().appearTime;
        if (appearProgress >= 1.0f)
        {
            castles.ForTeam(team).SpawnUnchecked(transform.position, cmsEnt);
            Destroy(gameObject);
        }

        foreach (var t in Tiles)
        {
            propBlock.Clear();
            ((CastleTile)t).constructState.spriteRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat("_time", appearProgress);
            ((CastleTile)t).constructState.spriteRenderer.SetPropertyBlock(propBlock);
        }

        base.Update();
    }
}