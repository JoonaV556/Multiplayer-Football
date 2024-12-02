using System.Collections.Generic;
using System.Linq;
using Fusion;
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
        public void OnBegun(NetworkRunner runner);

        /// <param name="NetworkDeltaTime">time between each update in seconds, determined by fixed network update rate</param>
        public void OnUpdate(float NetworkDeltaTime);

        public void OnEnd();

        public void OnBallEnteredGoal(Team GoalSide);

        public bool IsComplete();
    }

    public class WarmupPhase : IGamePhase
    {
        private float afterBallEnterGoalTimer;

        private bool ballTimerRunning = false;

        private NetworkRunner _runner;

        private List<KeyValuePair<PlayerRef, NetworkObject>> readyHandlers; // ready handlers track and update ready state for each player

        private NetworkObject readyHandlerPrefab;

        public void OnBegun(NetworkRunner runner)
        {
            _runner = runner;
            readyHandlerPrefab = GameManager.Instance.ReadyHandlerPrefab;

            // spawn ready handlers for each player
            readyHandlers = new();
            foreach (var player in runner.ActivePlayers)
            {
                readyHandlers.Add(
                    new KeyValuePair<PlayerRef, NetworkObject>(
                        player,
                        runner.Spawn(readyHandlerPrefab, Vector3.zero, Quaternion.identity, player)
                    )
                );
            }
        }

        public void OnUpdate(float NetworkDeltaTime)
        {
            UpdateBall(NetworkDeltaTime);

            if (readyHandlers != null)
                UpkeepReadyHandlers();
        }

        private void UpkeepReadyHandlers()
        {
            if (_runner == null) return;

            // spawn ready handlers for newly joining players
            foreach (var player in _runner.ActivePlayers)
            {
                if (!readyHandlers.Any(handler => handler.Key.Equals(player)))
                {
                    var handler = new KeyValuePair<PlayerRef, NetworkObject>(
                        player,
                        _runner.Spawn(readyHandlerPrefab, Vector3.zero, Quaternion.identity, player)
                    );
                    readyHandlers.Add(handler);
                }
            }
            // remove ready handlers for leaving players
            var toRemove = new List<KeyValuePair<PlayerRef, NetworkObject>>();
            foreach (var handler in readyHandlers)
            {
                if (!_runner.IsPlayerValid(handler.Key))
                {
                    toRemove.Add(handler);
                }
            }
            if (toRemove.Any())
            {
                foreach (var handler in toRemove)
                {
                    readyHandlers.Remove(handler);
                }
            }
        }

        private void UpdateBall(float NetworkDeltaTime)
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
            // despawn leftover handlers
            foreach (var handler in readyHandlers)
            {
                _runner.Despawn(handler.Value);
            }
            readyHandlers.Clear();
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
        public void OnBegun(NetworkRunner runner)
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
        public void OnBegun(NetworkRunner runner)
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
