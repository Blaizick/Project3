using System;
using System.Collections.Generic;
using UnityEngine;

public class CastleTile : Tile
{
    [NonSerialized] public Building build;
    
    [Serializable]
    public class State
    {
        public GameObject root;
        public SpriteRenderer spriteRenderer;
    }
    public State baseState;
    public State constructState;
    public List<State> AllStates => new(){baseState, constructState};

    public override void Init()
    {
        foreach (var s in AllStates)
        {
            s.spriteRenderer.sprite = cmsEnt.Get<CmsSpriteComp>().sprite;
        }
        // constructState.spriteRenderer.material.mainTexture = cmsEnt.Get<CmsSpriteComp>().sprite.texture;

        base.Init();
    }
}