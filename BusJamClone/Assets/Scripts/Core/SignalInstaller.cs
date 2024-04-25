using Zenject;

public class SignalInstaller : Installer<SignalInstaller>
{
    public override void InstallBindings()
    {
        Container.DeclareSignal<FingerDownSignal>().OptionalSubscriber();
        Container.DeclareSignal<FingerUpdateSignal>().OptionalSubscriber();
        Container.DeclareSignal<FingerUpSignal>().OptionalSubscriber();

        Container.DeclareSignal<GameStateChangedSignal>().OptionalSubscriber();
        Container.DeclareSignal<TimerChangedSignal>().OptionalSubscriber();
        Container.DeclareSignal<TimesUpSignal>().OptionalSubscriber();
        Container.DeclareSignal<SavedGameLoadedSignal>().OptionalSubscriber();

        SignalBusInstaller.Install(Container);
    }
}