using BJect;
using UnityEngine;

public static class InjectTags
{
}

public class ResolverMain : Resolver
{
    public DesktopInput input;
    public Player player;
    public Ui ui;

    public override void Resolve()
    {
        Container.Bind<Building.Factory>().AsSingle();
        Container.Bind<Tile.Factory>().AsSingle();
        Container.Bind<Castle.Factory>().AsSingle();
        Container.Bind<Projectile.Factory>().AsSingle();

        Container.Bind<DesktopInput>().FromInstance(input).AsSingle();

        Container.Bind<ResourcesSystem>().AsSingle();
        Container.Bind<Player>().AsSingle();
        Container.Bind<EnemySpawner>().AsSingle();

        Container.Bind<Ui>().FromInstance(ui).AsSingle();
    }
}