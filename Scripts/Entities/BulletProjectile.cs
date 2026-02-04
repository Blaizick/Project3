using UnityEngine;

public class BulletProjectile : Projectile
{
    public override void Init()
    {
        rb.linearVelocity = transform.right * cmsEnt.Get<CmsMoveSpeedComp>().moveSpeed;
        transform.rotation = Quaternion.identity;
        base.Init();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IHealthComp>(out var h) && 
            collision.TryGetComponent<TeamComp>(out var t) && 
            t.team.IsEnemy(teamComp.team) &&
            h.CanDamage)
        {
            h.TakeDamage(cmsEnt.Get<CmsDamageComp>().damage);
            Destroy(gameObject);
        }
    }
}