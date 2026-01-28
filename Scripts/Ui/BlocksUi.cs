using System;
using System.Collections.Generic;
using BJect;
using UnityEngine;

public class BlocksUi : MonoBehaviour
{
    public RectTransform contentRootTr;

    public BlockUiPfb blockUiPfb;
    [Inject] public Container container;
    [Inject] public DesktopInput input;

    [NonSerialized] public List<BlockUiPfb> instances = new();

    public void Init()
    {
        Rebuild();
    }

    public void Update()
    {
        
    }

    public void Rebuild()
    {
        instances.ForEach(i => Destroy(i.gameObject));
        instances.Clear();

        foreach (var b in Blocks.all)
        {
            var scr = container.Instantiate(blockUiPfb, contentRootTr);
            foreach (var s in scr.AllStates)
            {
                s.btn.onClick.AddListener(() =>
                {
                    input.SelectSPlan(b);
                });
            }            
            instances.Add(scr);
        }
    }
}