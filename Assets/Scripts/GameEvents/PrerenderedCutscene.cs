
public class PrerenderedCutscene : GameEvent
{ 
    //the video to play

    public PrerenderedCutscene() { }

    public override void EndGameEvent(GameEventContext context)
    {
        throw new System.NotImplementedException();
    }

    public override void StartGameEvent(GameEventContext context)
    {
        //base.StartGameEvent(context);
        //start playing video
    }
}