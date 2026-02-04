using System;
using System.Collections.Generic;
using BJect;
using UnityEngine;

public class BlocksUi : MonoBehaviour, IControlMenu
{
    public GameObject root;
    public GameObject Root {get => root; set => root = value;}
    public RectTransform contentRootTr;

    public BlockUiPfb blockUiPfb;
    [Inject, NonSerialized] public DiContainer container;
    [Inject, NonSerialized] public DesktopInput input;

    [NonSerialized] public List<BlockUiPfb> instances = new();
    [Inject, NonSerialized] public UnlocksSystem unlocks;

    public void Init()
    {
        Rebuild();
        unlocks.onChange.AddListener(() => Rebuild());
    }

    public void Update()
    {
        
    }

    public void Rebuild()
    {
        instances.ForEach(i => Destroy(i.gameObject));
        instances.Clear();

        foreach (var b in unlocks.GetUnlockedBlocks())
        {
            var scr = container.Instantiate(blockUiPfb, contentRootTr);
            foreach (var s in scr.AllStates)
            {
                s.btn.onClick.AddListener(() =>
                {
                    input.SelectSPlan(b);
                });
                s.img.sprite = b.Get<CmsSpriteComp>().sprite;
            }            
            instances.Add(scr);
        }
    }
}