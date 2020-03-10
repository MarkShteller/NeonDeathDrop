
public abstract class GameEvent 
{
    internal GameManager gameManager;

    public GameEvent()
    {
        gameManager = GameManager.Instance;
    }

    public virtual void StartGameEvent(GameEventContext context) { }
    public virtual void EndGameEvent(GameEventContext context) { }
}

public class GameEventContext
{ }
