using BJect;
using UnityEngine;

public class Player
{
    [Inject] public Castle.Factory castleFactory;
    public Castle castle;

    public void Init()
    {
        castle = castleFactory.Use(Cms.Get("PlayerCastle0"), Teams.ally);
    }

    public void Update()
    {
        
    }
}