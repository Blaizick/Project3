using System;
using BJect;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class DesktopInput : MonoBehaviour
{
    public InputSystem_Actions actions;

    public Vector2 mousePos;
    public Vector2 mouseWorldPos;

    public SpriteRenderer sPlanSpriteRenderer;
    public CmsEnt sPlanBlock;
    public bool canPlaceSPlan;

    public Tile tileUnderCursor;

    public RectTransform breakSelectionImgTr;
    public GameObject breakSelectionImgRoot;
    [NonSerialized] public Vector2 breakSelection0;
    [NonSerialized] public Vector2 breakSelection1;
    [NonSerialized] public bool breaking;

    [NonSerialized, Inject] public BlockTooltipUi blockTooltipUi;

    public void Init()
    {
        actions = new();
        actions.Enable();
    }

    public void Update()
    {
        mousePos = actions.Player.MousePosition.ReadValue<Vector2>();
        mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

        tileUnderCursor = null;
        var mouseHits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero);
        canPlaceSPlan = false;
        sPlanSpriteRenderer.gameObject.SetActive(false);
        if (sPlanBlock)
        {
            foreach (var i in mouseHits)
            {
                if (i.transform.gameObject.TryGetComponent<Castle>(out var castle) && castle.teamComp.team.IsAlly(Teams.ally))
                {
                    var t = castle.grid.GetTileAt(mouseWorldPos);
                    if (t != null)
                    {
                        var sPlanSize = sPlanBlock.Get<CmsGridSizeComp>();

                        Vector2Int anchor = GeometryUtils.GetBuildAnchor(t.pos, sPlanSize.size);

                        if (t.grid.CanPlace(anchor, sPlanBlock.Get<CmsGridSizeComp>().size))
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

        if (actions.Player.Place.IsPressed())
        {
            if (canPlaceSPlan && 
                tileUnderCursor && 
                sPlanBlock && 
                tileUnderCursor.grid.HasAllResourcesForBlock(sPlanBlock))
            {
                PlaceSPlan();
            }
        }

        if (actions.Player.CameraMoveBtn.IsPressed())
        {
            Vector2 mov = -actions.Player.CameraMove.ReadValue<Vector2>() * 0.003f * Camera.main.orthographicSize;
            Camera.main.transform.position += (Vector3)mov;
        }

        if (actions.Player.Break.IsPressed())
        {
            if (sPlanBlock)
            {
                DeselectSPlan();
            }
            else
            {
                breakSelection1 = mousePos;
                if (!breaking)
                {
                    breakSelection0 = mousePos;
                }
                breaking = true;
                breakSelectionImgTr.anchoredPosition = (breakSelection0 + breakSelection1) * 0.5f;
                Vector2 min = new Vector2(Mathf.Min(breakSelection0.x, breakSelection1.x), Mathf.Min(breakSelection0.y, breakSelection1.y));
                Vector2 max = new Vector2(Mathf.Max(breakSelection0.x, breakSelection1.x), Mathf.Max(breakSelection0.y, breakSelection1.y));
                breakSelectionImgTr.sizeDelta = max - min;
            }
        }
        if (actions.Player.Break.WasReleasedThisFrame())
        {
            breaking = false;
        
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
        breakSelectionImgRoot.SetActive(breaking);

        float zoomDt = -actions.Player.CameraZoom.ReadValue<float>() * 0.5f;
        Camera.main.orthographicSize += zoomDt;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 2, 10);    
    
        blockTooltipUi.root.SetActive(sPlanBlock);
        if (sPlanBlock)
        {
            blockTooltipUi.Set(BlockUtils.GetTooltipTitle(sPlanBlock), BlockUtils.GetDesc(sPlanBlock));
        }
    }

    public void PlaceSPlan()
    {
        Vector2Int pos = GeometryUtils.GetBuildAnchor(tileUnderCursor.pos, sPlanBlock.Get<CmsGridSizeComp>().size);
        tileUnderCursor.grid.StartConstructing(sPlanBlock, pos);
    }

    public void SelectSPlan(CmsEnt block)
    {
        sPlanBlock = block;
        sPlanSpriteRenderer.sprite = block.Get<CmsSpriteComp>().sprite;
    }
    public void DeselectSPlan()
    {
        sPlanBlock = null;
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