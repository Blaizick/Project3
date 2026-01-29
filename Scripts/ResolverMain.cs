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
    public BlocksUi blocksUi;
    public BlockTooltipUi blockTooltipUi;

    public override void Resolve()
    {
        Container.Bind<Tile.Factory>().AsSingle();
        Container.Bind<Castle.Factory>().AsSingle();
        Container.Bind<Projectile.Factory>().AsSingle();

        Container.Bind<DesktopInput>().FromInstance(input).AsSingle();

        Container.Bind<ResourcesSystem>().AsSingle();
        Container.Bind<Player>().AsSingle();
        Container.Bind<EnemySpawner>().AsSingle();

        Container.Bind<BlocksUi>().FromInstance(blocksUi).AsSingle();
        Container.Bind<BlockTooltipUi>().FromInstance(blockTooltipUi).AsSingle();
        Container.Bind<Ui>().FromInstance(ui).AsSingle();
    }
}