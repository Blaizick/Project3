using System;
using System.Collections.Generic;
using BJect;
using UnityEngine;

public class EntitiesControlUi : MonoBehaviour
{
    [NonSerialized, Inject] public DiContainer container;

    public ControlledEntityUiPfb controlledEntityUiPfb;

    public GameObject root;

    public GameObject buildsRoot;
    public RectTransform buildsContentTr;
    public GameObject castlesRoot;
    public RectTransform castlesContentTr;

    [NonSerialized] public List<ControlledEntityUiPfb> buildInstances = new();
    [NonSerialized] public List<ControlledEntityUiPfb> castleInstances = new();

    [NonSerialized, Inject] public DesktopInput input;

    public void Init()
    {
        input.onControlledCastlesChange.AddListener(RebuildCastles);
        input.onControlledBuildsChange.AddListener(RebuildBuilds);
    }

    public void Update()
    {
        castlesRoot.SetActive(!input.controlBuildsMode);
        buildsRoot.SetActive(input.controlBuildsMode);
    }

    public void RebuildCastles()
    {
        castleInstances.ForEach(i => Destroy(i.gameObject));
        castleInstances.Clear();

        foreach (var c in input.controlledCastles)
        {
            var scr = container.Instantiate(controlledEntityUiPfb, castlesContentTr);
            castleInstances.Add(scr);
        }
    }
    public void RebuildBuilds()
    {
        buildInstances.ForEach(i => Destroy(i.gameObject));
        buildInstances.Clear();

        foreach (var i in input.controlledBuilds)
        {
            var scr = container.Instantiate(controlledEntityUiPfb, buildsContentTr);
            buildInstances.Add(scr);
        }
    }
}