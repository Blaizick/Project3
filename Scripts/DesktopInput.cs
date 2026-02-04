using System;
using System.Collections.Generic;
using System.ComponentModel;
using BJect;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum InputState
{
    Place,
    Control,
    PlaceCastle,
}

public class DesktopInput : MonoBehaviour
{
    [NonSerialized] public InputSystem_Actions actions;

    [NonSerialized] public InputState state;

    [NonSerialized] public Vector2 mousePos;
    [NonSerialized] public Vector2 mouseWorldPos;
    [NonSerialized] public Tile tileUnderCursor;

    public SpriteRenderer sPlanSpriteRenderer;
    [NonSerialized] public CmsEnt sPlanBlock;
    [NonSerialized] public bool canPlaceSPlan;

    public RectTransform breakSelectionImgTr;
    public GameObject breakSelectionImgRoot;
    [NonSerialized] public Vector2 breakSelection0;
    [NonSerialized] public Vector2 breakSelection1;
    [NonSerialized] public bool breaking;

    [NonSerialized, Inject] public BlockTooltipUi blockTooltipUi;

    [NonSerialized] public bool controlSelection;
    [NonSerialized] public Vector2 controlSelection0;
    [NonSerialized] public Vector2 controlSelection1;
    [NonSerialized] public List<Castle> controlledCastles = new();
    [NonSerialized] public List<Building> controlledBuilds = new();
    public RectTransform controlSelectionImgTr;
    public GameObject controlSelectionImgRoot;
    [NonSerialized] public UnityEvent onControlledCastlesChange = new();
    [NonSerialized] public UnityEvent onControlledBuildsChange = new();

    [NonSerialized, Inject] public BuildingsSystem buildings;
    [NonSerialized, Inject] public CastlesSystem castles;
    [NonSerialized, Inject] public Ui ui;
    [NonSerialized, Inject] public DiContainer container;

    /// <summary>
    /// Can control whether buildings or castles
    /// </summary>
    [NonSerialized] public bool controlBuildsMode;

    [NonSerialized] public BGrid sCastleGrid;
    [NonSerialized] public CmsEnt sCastleEnt;

    [NonSerialized] public bool pointerOverUi;

    public void Init()
    {
        actions = new();
        actions.Enable();

        state = InputState.Control;
    }

    public void Update()
    {
        mousePos = actions.Player.MousePosition.ReadValue<Vector2>();
        mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

        var mouseHits = Physics2D.OverlapPointAll(mouseWorldPos);

        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position = mousePos}, results);
            pointerOverUi = false;
            foreach (var i in results)
            {
                if (i.gameObject != null && LayerMasksUtils.ContainsLayer(LayerMasks.uiMask, i.gameObject.layer))
                {
                    pointerOverUi = true;
                    break;
                }
            }
        }

        int count = controlledCastles.RemoveAll(i => i == null);
        if (count > 0)
        {
            onControlledCastlesChange?.Invoke();
        }
        count = controlledBuilds.RemoveAll(i => i == null);
        if (count > 0)
        {
            onControlledBuildsChange?.Invoke();
        }

        if (actions.Player.Control.WasPerformedThisFrame())
        {
            state = InputState.Control;

            controlledBuilds.Clear();
            controlledCastles.Clear();

            onControlledCastlesChange?.Invoke();
            onControlledBuildsChange?.Invoke();
        }

        if (actions.Player.SwitchControlBlocksState.WasPerformedThisFrame())
        {
            controlBuildsMode = !controlBuildsMode;
        }

        tileUnderCursor = null;
        canPlaceSPlan = false;
        sPlanSpriteRenderer.gameObject.SetActive(false);

        if (state == InputState.Place)
        {
            foreach (var i in mouseHits)
            {
                if (i.TryGetComponent<Castle>(out var castle) && castle.teamComp.team.IsAlly(Teams.ally))
                {
                    var t = castle.grid.GetTileAt(mouseWorldPos);
                    if (t != null)
                    {
                        var sPlanSize = sPlanBlock.Get<CmsGridSizeComp>();

                        Vector2Int anchor = GeometryUtils.GetBuildAnchor(t.pos, sPlanSize.size);

                        if (((CastleGrid)t.grid).CanPlace(anchor, sPlanBlock.Get<CmsGridSizeComp>().size))
                        {
                            canPlaceSPlan = true;

                            sPlanSpriteRenderer.gameObject.SetActive(true);

                            sPlanSpriteRenderer.transform.position = 
                                castle.grid.GetBuildPosition(anchor, sPlanSize.size);
                        }
            
                        tileUnderCursor = t;    
                    }
                }
            }
        }

        if (state != InputState.Control)
        {
            controlSelection = false;
        }
        if (actions.Player.Place.WasPerformedThisFrame())
        {
            if (state == InputState.Control)
            {
                if (!pointerOverUi)
                {
                    controlSelection = true;
                    controlSelection0 = mousePos;    
                }
            }
        }
        if (actions.Player.Place.IsPressed())
        {
            if (state == InputState.Control)
            {
                if (controlSelection)
                {
                    controlSelection1 = mousePos;

                    Vector2 min = new Vector2(Mathf.Min(controlSelection0.x, controlSelection1.x), Mathf.Min(controlSelection0.y, controlSelection1.y));            
                    Vector2 max = new Vector2(Mathf.Max(controlSelection0.x, controlSelection1.x), Mathf.Max(controlSelection0.y, controlSelection1.y));            

                    controlSelectionImgTr.anchoredPosition = (min + max) * 0.5f;
                    controlSelectionImgTr.sizeDelta = max - min;
                }
            }
            else if (state == InputState.Place)
            {
                if (!pointerOverUi)
                {
                    if (canPlaceSPlan && 
                        tileUnderCursor && 
                        sPlanBlock && 
                        ((CastleGrid)tileUnderCursor.grid).HasAllResourcesForBlock(sPlanBlock))
                    {
                        PlaceSPlan();
                    }    
                }
            }
            else if (state == InputState.PlaceCastle)
            {
                if (!pointerOverUi)
                {
                    if (castles.ForTeam(Teams.ally).CanPlaceOne())
                    {
                        PlaceSCastle();
                    }    
                }
            }
        }
        if (actions.Player.Place.WasReleasedThisFrame())
        {
            if (controlSelection)
            {
                if (controlBuildsMode)
                {
                    controlledBuilds.Clear();
                }
                else
                {
                    controlledCastles.Clear();
                }

                Vector2 w0 = Camera.main.ScreenToWorldPoint(controlSelection0);
                Vector2 w1 = Camera.main.ScreenToWorldPoint(controlSelection1);

                Vector2 min = new Vector2(Mathf.Min(w0.x, w1.x), Mathf.Min(w0.y, w1.y));            
                Vector2 max = new Vector2(Mathf.Max(w0.x, w1.x), Mathf.Max(w0.y, w1.y));            

                var hits = Physics2D.OverlapBoxAll((min + max) * 0.5f, max - min, 0);
                foreach (var hit in hits)
                {
                    if (controlBuildsMode)
                    {
                        if (hit.TryGetComponent<Building>(out var b) &&
                            b.teamComp.team.IsAlly(Teams.ally))
                        {
                            controlledBuilds.Add(b);
                        }
                    }
                    else
                    {
                        if (hit.TryGetComponent<Castle>(out var c) &&
                            c.teamComp.team.IsAlly(Teams.ally))
                        {
                            controlledCastles.Add(c);
                        }    
                    }
                }

                if (controlBuildsMode)
                {
                    onControlledBuildsChange?.Invoke();
                }
                else
                {
                    onControlledCastlesChange?.Invoke();
                }
                controlSelection = false;
            }
        }
        controlSelectionImgRoot.SetActive(controlSelection);


        if (actions.Player.CameraMoveBtn.IsPressed())
        {
            Vector2 mov = -actions.Player.CameraMove.ReadValue<Vector2>() * 0.003f * Camera.main.orthographicSize;
            Camera.main.transform.position += (Vector3)mov;
        }

        if (actions.Player.Break.WasPerformedThisFrame())
        {
            if (state == InputState.PlaceCastle)
            {
                if (!pointerOverUi)
                {
                    DeselectSCastle();
                }
            }
            else if (state == InputState.Place)
            {
                if (!pointerOverUi)
                {
                    DeselectSPlan();
                }
            }
            else if (state == InputState.Control)
            {
                if (!pointerOverUi)
                {
                    if (controlBuildsMode)
                    {
                        if (controlledBuilds.Count > 0)
                        {

                        }
                        else
                        {
                            breakSelection0 = mousePos;
                            breaking = true;
                        }
                    }
                    else
                    {
                        if (controlledCastles.Count > 0)
                        {
                            foreach (var i in controlledCastles)
                            {
                                i.SetMovePos(mouseWorldPos);
                            }
                        }
                        else
                        {
                            breakSelection0 = mousePos;
                            breaking = true;        
                        }
                    }    
                }
            }
        }
        if (actions.Player.Break.IsPressed())
        {
            breakSelection1 = mousePos;

            if (breaking)
            {
                breakSelectionImgTr.anchoredPosition = (breakSelection0 + breakSelection1) * 0.5f;
                Vector2 min = new Vector2(Mathf.Min(breakSelection0.x, breakSelection1.x), Mathf.Min(breakSelection0.y, breakSelection1.y));
                Vector2 max = new Vector2(Mathf.Max(breakSelection0.x, breakSelection1.x), Mathf.Max(breakSelection0.y, breakSelection1.y));
                breakSelectionImgTr.sizeDelta = max - min;    
            }
        }
        if (actions.Player.Break.WasReleasedThisFrame())
        {
            if (breaking)
            {
                var tmp0 = Camera.main.ScreenToWorldPoint(breakSelection0);
                var tmp1 = Camera.main.ScreenToWorldPoint(breakSelection1);
            
                Vector2 min = new Vector2(Mathf.Min(tmp0.x, tmp1.x), Mathf.Min(tmp0.y, tmp1.y));
                Vector2 max = new Vector2(Mathf.Max(tmp0.x, tmp1.x), Mathf.Max(tmp0.y, tmp1.y));
            
                var pos = (tmp0 + tmp1) * 0.5f;
                var size = max - min;

                var hits = Physics2D.OverlapBoxAll(pos, size, 0.0f);
                foreach (var hit in hits)
                {
                    if (hit.TryGetComponent<Building>(out var b) &&
                        b.CanBreak() &&
                        b.teamComp.team.IsAlly(Teams.ally))
                    {
                        b.grid.BreakBuild(b);
                    }
                }    
            }
            breaking = false;
        }
        breakSelectionImgRoot.SetActive(breaking);

        float zoomDt = -actions.Player.CameraZoom.ReadValue<float>() * 0.5f;
        Camera.main.orthographicSize += zoomDt;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 2, 20);    
    
        blockTooltipUi.root.SetActive(sPlanBlock);
        if (sPlanBlock)
        {
            blockTooltipUi.Set(BlockUtils.GetTooltipTitle(sPlanBlock), BlockUtils.GetBuildDesc(sPlanBlock));
        }

        if (state == InputState.PlaceCastle)
        {
            sCastleGrid.transform.position = mouseWorldPos;
        }
        
        foreach (var i in castles.ForTeam(Teams.ally).all)
        {
            if (i && i.controlFrameRoot)
            {
                i.controlFrameRoot.SetActive(false);
            }
        }
        foreach (var i in buildings.ForTeam(Teams.ally).all)
        {
            if (i && i.controlFrameRoot)
            {
                i.controlFrameRoot.SetActive(false);
            }
        }
        if (state == InputState.Control)
        {
            if (controlBuildsMode)
            {
                foreach (var i in buildings.ForTeam(Teams.ally).all)
                {
                    if (i && i.controlFrameRoot)
                    {
                        i.controlFrameRoot.SetActive(controlledBuilds.Contains(i));
                    }   
                }
            }
            else
            {
                foreach (var i in castles.ForTeam(Teams.ally).all)
                {
                    if (i && i.controlFrameRoot)
                    {
                        i.controlFrameRoot.SetActive(controlledCastles.Contains(i));
                    }       
                }
            }
        }
        
    }

    public void PlaceSPlan()
    {
        Vector2Int pos = GeometryUtils.GetBuildAnchor(tileUnderCursor.pos, sPlanBlock.Get<CmsGridSizeComp>().size);
        ((CastleGrid)tileUnderCursor.grid).StartConstructing(sPlanBlock, pos);
    }

    public void SelectSPlan(CmsEnt block)
    {
        state = InputState.Place;
        sPlanBlock = block;
        sPlanSpriteRenderer.sprite = block.Get<CmsSpriteComp>().sprite;
    }
    public void DeselectSPlan()
    {
        state = InputState.Control;
        sPlanBlock = null;
    }

    public void PlaceSCastle()
    {
        var c = castles.ForTeam(Teams.ally);

        if (c.castlesStored > 0 && 
            c.all.Count < c.maxCastles && 
            c.CanSpawn(mouseWorldPos, sCastleEnt))
        {
            c.castlesStored--;
            c.Spawn(mouseWorldPos, sCastleEnt);
        }
    }
    public void SelectSCastle(CmsEnt cmsEnt)
    {
        var grid = container.Instantiate(Profiles.playerCastlesProfile.Get<CmsGridPfbComp>().gridPfb);
        grid.Set(cmsEnt);
        grid.Init();
        sCastleGrid = grid;
        sCastleEnt = cmsEnt;
        state = InputState.PlaceCastle;
    }
    public void DeselectSCastle()
    {
        state = InputState.Control;
        Destroy(sCastleGrid.gameObject);
    }

    public void OnDrawGizmos()
    {
        if (tileUnderCursor != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(tileUnderCursor.transform.position, Vector3.one);
        }
    }
}

[Serializable]
public class CmsPfbComp : CmsComp
{
    public GameObject pfb;
}

[Serializable]
public class CmsSpriteComp : CmsComp
{
    public Sprite sprite;
}