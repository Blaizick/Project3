using BJect;
using UnityEngine;

public class Player
{
    public Team team;
    [Inject] public CastlesSystem castles;

    public void Init()
    {
        team = Teams.ally;
        castles.ForTeam(team).SpawnUnchecked(Vector2.zero, Castles.playerCastle0);
    }

    public void Update()
    {
        
    }
}