using System;
using BJect;
using UnityEngine;

public class Castle : MonoBehaviour
{
    [NonSerialized] public CmsEnt cmsEnt;

    public BoxCollider2D col;
    public Rigidbody2D rb;

    public CastleGrid grid;

    public TeamComp teamComp;
    public HealthComp healthComp;

    [NonSerialized, Inject] public CastlesSystem castles;

    [NonSerialized] public Vector2 movePos;
    [NonSerialized] public bool moving;

    public GameObject controlFrameRoot;
    public LineRenderer lineRenderer;

    [NonSerialized] public bool controlling; 
    [NonSerialized] public bool controllingCastles;

    [NonSerialized, Inject] public CastleDisappearSystem castleDisappearSystem;

    public void SetMovePos(Vector2 pos)
    {
        movePos = pos;
        moving = true;
    }

    public virtual void Init()
    {
        healthComp.Set(cmsEnt);
        healthComp.Init();
    
        healthComp.onDie.AddListener(() => 
        {
            castles.Remove(this);
            castleDisappearSystem.Disappear(this);
            Destroy(gameObject);
        });

        if (controlFrameRoot)
        {
            controlFrameRoot.SetActive(false);
        }

        foreach (var tile in grid.Tiles)
        {
            ((CastleTile)tile).baseState.root.SetActive(true);
            ((CastleTile)tile).constructState.root.SetActive(false);
        }
    }

    public virtual void Update()
    {
        if (moving)
        {
            if (MathUtils.IsWithin(transform.position, movePos, 0.05f))
            {
                moving = false;
            }
            rb.linearVelocity = (movePos - (Vector2)transform.position).normalized * cmsEnt.Get<CmsMoveSpeedComp>().moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;            
        }

        healthComp.canDamage = !grid.HasBuilding();
    
        if (lineRenderer)
        {
            if (moving && controllingCastles)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPositions(new Vector3[]
                {
                    transform.position,
                    movePos,
                });
            }
            else
            {
                lineRenderer.positionCount = 0;
            }
        }
        if (controlFrameRoot)
        {
            controlFrameRoot.SetActive(controllingCastles && controlling);
        }
    }
}

[Serializable]
public class CmsCastlePfbComp : CmsComp
{
    public Castle pfb;
}

[Serializable]
public class CmsShadowPfbComp : CmsComp
{
    public GameObject shadowPfb;
}