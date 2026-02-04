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
    public StaticTooltip staticTooltip;
    public CastleAssemblerUi castleAssemblerUi;
    public EntitiesControlUi entitiesControlMenu;
    public LayerMasks layerMasks;
    public MapSystem map;

    public override void Resolve()
    {
        Container.Bind<CastleTile.Factory>().AsSingle();
        Container.Bind<Projectile.Factory>().AsSingle();
        Container.Bind<Castle.Factory>().AsSingle();

        Container.Bind<LayerMasks>().FromInstance(layerMasks).AsSingle();
        Container.Bind<MapSystem>().FromInstance(map).AsSingle();
        Container.Bind<ResourcesSystem>().AsSingle();
        Container.Bind<Player>().AsSingle();
        Container.Bind<BuildingsSystem>().AsSingle();
        Container.Bind<CastlesSystem>().AsSingle();
        Container.Bind<EnemySpawner>().AsSingle();
        Container.Bind<UnlocksSystem>().AsSingle();
        Container.Bind<CapturePointSystem>().AsSingle();
        Container.Bind<CastleDisappearSystem>().AsSingle();

        Container.Bind<BlocksUi>().FromInstance(blocksUi).AsSingle();
        Container.Bind<BlockTooltipUi>().FromInstance(blockTooltipUi).AsSingle();
        Container.Bind<StaticTooltip>().FromInstance(staticTooltip).AsSingle();
        Container.Bind<CastleAssemblerUi>().FromInstance(castleAssemblerUi).AsSingle();
        Container.Bind<EntitiesControlUi>().FromInstance(entitiesControlMenu).AsSingle();
        Container.Bind<Ui>().FromInstance(ui).AsSingle();

        Container.Bind<DesktopInput>().FromInstance(input).AsSingle();
    }
}