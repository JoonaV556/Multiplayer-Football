using Fusion.Addons.Physics;
using UnityEngine;

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
            - once complete, manager runs OnEnd() and continues to next phase
    */

    public interface IGamePhase
    {
        public void OnBegun();

        /// <param name="NetworkDeltaTime">time between each update in seconds, determined by fixed network update rate</param>
        public void OnUpdate(float NetworkDeltaTime);

        public void OnEnd();

        public void OnBallEnteredGoal(Team GoalSide);

        public bool IsComplete();
    }

    public class WarmupPhase : IGamePhase
    {
        private bool playersReady = false;
        float afterBallEnterGoalTimer;
        bool ballTimerRunning = false;

        public void OnBegun()
        {
            // place all players in spawn positions

            // drop ball in center
        }

        public void OnUpdate(float NetworkDeltaTime)
        {
            if (ballTimerRunning)
            {
                afterBallEnterGoalTimer += NetworkDeltaTime;
            }

            var shouldResetBall = ballTimerRunning
            && afterBallEnterGoalTimer >= GameManager.Instance.AfterBallEnterGoalDelay;

            if (shouldResetBall)
            {
                ballTimerRunning = false;
                ResetBall();
            }
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

        public void OnBallEnteredGoal(Team GoalSide)
        {
            afterBallEnterGoalTimer = 0f;
            ballTimerRunning = true;
            TypeLogger.TypeLog(this, "ball entered goal during warmup", 1);
        }

        private void ResetBall()
        {
            var ball = GameManager.Instance.GetBall();
            var ballRigid = ball.GetComponent<Rigidbody>();
            var ballNetworkRigid = ball.GetComponent<NetworkRigidbody3D>();

            // stop movement
            ballRigid.linearVelocity = Vector3.zero;
            ballRigid.angularVelocity = Vector3.zero;
            // place in spawn pos
            ballNetworkRigid.Teleport(GameManager.Instance.BallSpawnPosition);
            TypeLogger.TypeLog(this, "Respawned ball", 1);
        }
    }

    public class MatchPhase : IGamePhase
    {
        public void OnBegun()
        {
            TypeLogger.TypeLog(this, "begun match phase", 1);
        }

        public void OnUpdate(float NetworkDeltaTime)
        {

        }

        public void OnEnd()
        {

        }

        public bool IsComplete()
        {
            return false;
        }

        public void OnBallEnteredGoal(Team GoalSide)
        {
        }
    }

    public class EndPhase : IGamePhase
    {
        public void OnBegun()
        {

        }

        public void OnUpdate(float NetworkDeltaTime)
        {

        }

        public void OnEnd()
        {

        }

        public bool IsComplete()
        {
            return false;
        }

        public void OnBallEnteredGoal(Team GoalSide)
        {
        }
    }

}
