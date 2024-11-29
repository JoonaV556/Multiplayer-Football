namespace FootBall
{

    /*
        phases are specific stages of game 
        manager runs through each game phase in order
        kind of like game states

        lifecycle of phase:
        - Phase is started
        - Phase updates continuosly
        - phase is ended when its IsCompleted returns true

        phase is considered complete when IsComplete returns true
            - once complete, manager continues to next phase
    */

    public interface IGamePhase
    {
        public void OnBegun();

        public void OnUpdate();

        public void OnEnd();

        public bool IsComplete();
    }

    public class WarmupPhase : IGamePhase
    {
        private bool playersReady = false;

        public void OnBegun()
        {
            // place all players in spawn positions

            // drop ball in center
        }

        public void OnUpdate()
        {

        }

        public void OnEnd()
        {

        }

        public bool IsComplete()
        {
            // conditions 
            // atleast 2 players present 
            // all players have stood in ready pos for enough time
            return false;
        }
    }

    public class MatchPhase : IGamePhase
    {
        public void OnBegun()
        {

        }

        public void OnUpdate()
        {

        }

        public void OnEnd()
        {

        }

        public bool IsComplete()
        {
            return false;
        }
    }

    public class EndPhase : IGamePhase
    {
        public void OnBegun()
        {

        }

        public void OnUpdate()
        {

        }

        public void OnEnd()
        {

        }

        public bool IsComplete()
        {
            return false;
        }
    }

}
