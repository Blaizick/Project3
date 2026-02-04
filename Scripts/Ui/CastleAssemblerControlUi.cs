using System;
using System.Linq;
using BJect;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CastleAssemblerUi : MonoBehaviour, IControlMenu
{
    public GameObject root;
    public GameObject Root { get => root; set => root = value; }

    public Button spawnBtn;
    public Button craftBtn;
    public Button closeBtn;

    [Inject, NonSerialized] public DesktopInput input;

    public void Init()
    {
        closeBtn.onClick.AddListener(() =>
        {
            input.state = InputState.Control;
        });
        craftBtn.onClick.AddListener(() =>
        {
            ((CastleAssembler)input.controlledBuilds.First()).StartCrafting();
        });
        spawnBtn.onClick.AddListener(() =>
        {
            input.SelectSCastle(Castles.playerCastle0);
            // input.state = InputState.PlaceCastle;
        });
    }

    public void Update()
    {
        
    }
}