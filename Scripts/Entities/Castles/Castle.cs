using System;
using BJect;
using UnityEngine;
using UnityEngine.Events;

public class Castle : MonoBehaviour
{
    [NonSerialized] public CmsEnt cmsEnt;

    public BoxCollider2D col;
    public Rigidbody2D rb;

    public CastleGrid grid;

    public TeamComp teamComp;
    public HealthComp healthComp;

    public UnityEvent onDestroy = new();

    [NonSerialized, Inject] public CastlesSystem castles;

    public Vector2 moveCastle;

    public Vector2 movePos;
    public bool moving;

    public GameObject controlFrameRoot;

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
            Destroy(gameObject);
        });

        if (controlFrameRoot)
        {
            controlFrameRoot.SetActive(false);
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
    }

    public class Factory : BaseFactory
    {
        public Factory(DiContainer container) : base(container)
        {
        }

        public Castle Use(Vector2 pos, CmsEnt cmsEnt, Team team)
        {
            var scr = container.Instantiate(cmsEnt.Get<CmsCastlePfbComp>().pfb);
            scr.cmsEnt = cmsEnt;
            scr.teamComp.team = team;
            
            scr.transform.position = pos;

            scr.grid.Set(cmsEnt, scr);
            
            Vector2 sizeV = new(scr.grid.size, scr.grid.size);
            scr.col.size = sizeV;
            scr.col.offset = Vector2.zero;
            
            scr.Init();
            scr.grid.Init();

            return scr;
        }
    }
}

[Serializable]
public class CmsCastlePfbComp : CmsComp
{
    public Castle pfb;
}