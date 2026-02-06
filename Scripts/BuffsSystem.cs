using System;
using System.Collections.Generic;
using BJect;

public class ModifiersSystem
{
    public DiContainer container;

    public Dictionary<Team, TeamModifiersSystem> modifierSystems = new();

    public ModifiersSystem(DiContainer container)
    {
        this.container = container;
    }

    public void Init()
    {
        foreach (var t in Teams.all)
        {
            modifierSystems[t] = container.Create<TeamModifiersSystem>(new(){t});
            modifierSystems[t].Init();
        }
    }

    public void Update()
    {
        foreach (var (k, v) in modifierSystems)
        {
            v.Update();
        }
    }
}

public class TeamModifiersSystem
{
    public Dictionary<Type, Modifier> modifiers = new();
    public Team team;

    public TeamModifiersSystem(Team team)
    {
        this.team = team;
    }

    public void Init()
    {
        
    }

    public void Update()
    {
        
    }

    public float GetBonus<T>() where T : IBonus
    {
        float b = 0;
        foreach (var (k, v) in modifiers)
        {
            if (v is T m)
            {
                b += m.Bonus;
            }
        }
        return b;
    }
    public float GetMultiplier<T>() where T : IMultiplier
    {
        float b = 0;
        foreach (var (k, v) in modifiers)
        {
            if (v is T m)
            {
                b += m.Multiplier;
            }
        }
        return b;
    }
}

public class Modifier
{
    
}

public interface IBonus
{
    public float Bonus { get; }
}

public interface IMultiplier
{
    public float Multiplier { get; }
}