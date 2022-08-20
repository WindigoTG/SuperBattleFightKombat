
public abstract class PlayerState
{
    public abstract void SetDependencies(PlayerModel model, PlayerView view, ContactsPoller contactPoller);

    public abstract void Activate();

    public abstract void Update(CurrentInputs inputs);

    public abstract void Attack();
}
