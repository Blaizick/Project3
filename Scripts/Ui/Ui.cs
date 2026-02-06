using System;
using System.Collections.Generic;
using System.Linq;
using BJect;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ControlModeMarker
{
    public State buildsState;
    public State castlesState;

    [Serializable]
    public class State
    {
        public GameObject root;
        public Button btn;
        public TMP_Text text;
    }

    public List<State> AllStates => new(){buildsState, castlesState,}; 
}

public class Ui : MonoBehaviour
{
    [Inject, NonSerialized] public BlocksUi blocksUi;

    public TMP_Text essenceText;
    public TMP_Text quintessenceText;

    [Inject, NonSerialized] public ResourcesSystem resources;

    public TMP_Text timeText;
    public TMP_Text timeLeftText;

    [Inject, NonSerialized] public BlockTooltipUi blockTooltipUi;
    [Inject, NonSerialized] public StaticTooltip staticTooltip;
    [Inject, NonSerialized] public CastleAssemblerUi castleAssemblerUi;

    public List<IControlMenu> allMenus = new();

    [Inject, NonSerialized] public DesktopInput input;

    [NonSerialized] public List<IControlMenu> menusStack = new();

    public TMP_Text castlesStoredText; 

    [NonSerialized, Inject] public EntitiesControlUi entitiesControlMenu;

    public ControlModeMarker controlModeMarker;

    [Inject, NonSerialized] public EnemySpawner enemySpawner;
    [Inject, NonSerialized] public CastlesSystem castles;

    public void Init()
    {
        blocksUi.Init();
        blockTooltipUi.Init();
        staticTooltip.Init();
        castleAssemblerUi.Init();
        entitiesControlMenu.Init();

        allMenus = new()
        {
            blocksUi, castleAssemblerUi,
        };

        foreach (var state in controlModeMarker.AllStates)
        {
            state.btn.onClick.AddListener(() => input.controlBuildsMode = !input.controlBuildsMode);
        }
    }


    public void Update()
    {
        essenceText.text = $"{CmsResources.essence.Get<CmsNameComp>().name}: {(int)resources.ForTeam(Teams.ally).Get(CmsResources.essence)}";
        quintessenceText.text = $"{CmsResources.quintessence.Get<CmsNameComp>().name}: {(int)resources.ForTeam(Teams.ally).Get(CmsResources.quintessence)}";

        timeText.text = TextUtils.TimeToString(Time.time);
        timeLeftText.text = TextUtils.TimeToString(enemySpawner.TimeLeft);

        castleAssemblerUi.root.SetActive(input.state == InputState.Control && 
            input.controlBuildsMode &&
            input.controlledBuilds.Count == 1 && 
            input.controlledBuilds.First() is CastleAssembler);
        blocksUi.root.SetActive(!castleAssemblerUi.root.activeInHierarchy);

        entitiesControlMenu.root.SetActive(input.state == InputState.Control &&
            ((input.controlBuildsMode && input.controlledBuilds.Count > 0) ||
            (!input.controlBuildsMode && input.controlledCastles.Count > 0)));

        castlesStoredText.text = castles.ForTeam(Teams.ally).castlesStored.ToString();
    
        controlModeMarker.buildsState.root.SetActive(input.controlBuildsMode);
        controlModeMarker.castlesState.root.SetActive(!input.controlBuildsMode);
    }
}

public interface IControlMenu
{
    GameObject Root {get; set;}
}