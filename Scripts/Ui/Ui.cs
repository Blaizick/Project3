using System;
using BJect;
using TMPro;
using UnityEngine;

public class Ui : MonoBehaviour
{
    [Inject, NonSerialized] public BlocksUi blocksUi;

    public TMP_Text essenceText;

    [Inject, NonSerialized] public ResourcesSystem resources;

    public TMP_Text timeText;

    [Inject, NonSerialized] public BlockTooltipUi blockTooltipUi;

    public void Init()
    {
        blocksUi.Init();
        blockTooltipUi.Init();
    }

    public void Update()
    {
        essenceText.text = ((int)resources.Get(CmsResources.essence)).ToString();
        timeText.text = ((int)Time.time).ToString();
    }
}