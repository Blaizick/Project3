using System;
using UnityEngine;
using UnityEngine.Events;

public interface IHealthComp
{
    void Heal(float heal);
    void TakeDamage(float damage);
    bool CanDamage {get;}
}

public class HealthComp : MonoBehaviour, IHealthComp
{
    public float health;
    public float maxHealth;

    public UnityEvent onDie = new();
    public UnityEvent<float> onDamaged = new();
    public UnityEvent<float> onHealed = new();

    public bool canDamage = true;

    public void Set(CmsEnt cmsEnt)
    {
        maxHealth = cmsEnt.Get<CmsHealthComp>().maxHealth;
    }

    public void Init()
    {
        health = maxHealth;
    }

    public void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        onDamaged?.Invoke(damage);
        if (health <= 0)
        {
            onDie?.Invoke();
        }
    }
    public void Heal(float heal)
    {
        onHealed?.Invoke(heal);
        health = Mathf.Clamp(health + heal, 0, maxHealth);
    }

    public virtual bool CanDamage => canDamage;
}

[Serializable]
public class CmsHealthComp : CmsComp
{
    public float maxHealth;
}
