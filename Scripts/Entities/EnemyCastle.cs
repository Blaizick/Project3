using System;
using BJect;
using UnityEngine;

public class EnemyCastle : Castle
{
    [Inject] public Player player;

    public override void Update()
    {
        if (player.castle && player.castle.grid)
        {
            rb.linearVelocity = (player.castle.grid.transform.position - grid.transform.position).normalized * 2.0f;
        }
    
        base.Update();
    }
}

public class EnemySpawner
{
    public EnemyCastle.Factory factory;

    public float spawnProgress;

    public CmsEnt cmsEnt;

    public Player player;

    public EnemySpawner(EnemyCastle.Factory factory, Player player)
    {
        this.factory = factory;
        this.player = player;
    }

    public void Init()
    {
        cmsEnt = Cms.Get("EnemySpawner0");
    }

    public void Update()
    {
        if (!player.castle || !player.castle.grid)
        {
            return;
        }

        spawnProgress += Time.deltaTime / 2.0f;

        if (spawnProgress > 1.0f)
        {
            var p = factory.Use(Cms.Get("EnemyCastle0"), Teams.enemy);

            float min = cmsEnt.Get<CmsMinMaxDstComp>().minDst;
            float hMax = cmsEnt.Get<CmsMinMaxDstComp>().maxDst * 0.5f;
            
            Vector2 hMaxV = new Vector2(hMax, hMax);

            Vector2 aa = (Vector2)player.castle.grid.transform.position - hMaxV;
            Vector2 bb = (Vector2)player.castle.grid.transform.position + hMaxV;

            Vector2 pos;
            do
            {
                pos = new Vector2(UnityEngine.Random.Range(aa.x, bb.x), UnityEngine.Random.Range(aa.y, bb.y));
            }
            while (Vector2.Distance(pos, player.castle.grid.transform.position) < min);

            p.grid.transform.position = pos;
            p.grid.PlaceBlock(Blocks.turret0, new(0, 0));

            spawnProgress = 0.0f;
        }
    }
}

[Serializable]
public class CmsMinMaxDstComp : CmsComp
{
    public float minDst;
    public float maxDst;
}