using Zenject;

public class GameInstaller : MonoInstaller
{
    #region Zenject

    private ReferenceHolder _referenceHolder;

    [Inject]
    private void Construct(ReferenceHolder referenceHolder)
    {
        _referenceHolder = referenceHolder;
    }

    #endregion

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<InputController>().AsSingle();
        Container.BindInterfacesAndSelfTo<TimerController>().AsSingle();
        Container.BindInterfacesAndSelfTo<LevelController>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
        Container.BindInterfacesAndSelfTo<SaveController>().AsSingle();
        Container.BindInterfacesAndSelfTo<BusController>().AsSingle();
        Container.BindInterfacesAndSelfTo<BoardCoordinateSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<QueueController>().AsSingle();

        CreatePools();

        SignalInstaller.Install(Container);
    }


    private void CreatePools()
    {
        Container.BindFactory<GridSlot, GridSlot.Factory>().FromPoolableMemoryPool<GridSlot, GridSlot.Pool>(
            poolBinder => poolBinder.WithInitialSize(20).FromComponentInNewPrefab(_referenceHolder.GridSlotPrefab)
                .UnderTransformGroup("GridSlots"));

        Container.BindFactory<PassengerGridSlotObject, PassengerGridSlotObject.Factory>()
            .FromPoolableMemoryPool<PassengerGridSlotObject, PassengerGridSlotObject.Pool>(poolBinder =>
                poolBinder.WithInitialSize(10).FromComponentInNewPrefab(_referenceHolder.PassengerPrefab)
                    .UnderTransformGroup("Passengers"));

        Container.BindFactory<BusObject, BusObject.Factory>().FromPoolableMemoryPool<BusObject, BusObject.Pool>(
            poolBinder => poolBinder.WithInitialSize(3).FromComponentInNewPrefab(_referenceHolder.BusPrefab)
                .UnderTransformGroup("Buses"));

        Container.BindFactory<QueueSlot, QueueSlot.Factory>().FromPoolableMemoryPool<QueueSlot, QueueSlot.Pool>(
            poolBinder => poolBinder.WithInitialSize(3).FromComponentInNewPrefab(_referenceHolder.QueueSlotPrefab)
                .UnderTransformGroup("QueueSlots"));
    }
}