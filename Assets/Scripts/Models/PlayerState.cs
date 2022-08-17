
public abstract class PlayerState : IUpdateableRegular
{
    public abstract void SetDependencies(PlayerModel model, PlayerView view, ContactsPoller contactPoller);

    public abstract void Activate();

    public abstract void UpdateRegular();
}
