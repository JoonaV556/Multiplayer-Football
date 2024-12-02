using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.Rendering;
using static TypeLogger;

namespace FootBall
{
    [Serializable]
    public class SpawnPoint
    {
        public PlayerData _OccupyingPlayer;
        public Transform _Transform;
        public Team _Side;
    }

    /// <summary>
    /// Manages football game-related logic. Spawned for all players but management code runs only on server 
    /// </summary>
    public class GameManager : NetworkBehaviour, INetworkRunnerCallbacks
    {
        /*
        Responsibilities:
        - Spawning players
        - Team handling for players
        - Updating game state/phases (warmup, match etc.)
        - Handling ball, goals etc.
        */

        public static GameManager Instance;

        public NetworkObject PlayerPrefab;

        public NetworkObject ReadyHandlerPrefab;

        public NetworkObject BallPrefab;

        public Vector3 BallSpawnPosition = Vector3.zero;

        public SpawnPoint[] SpawnPoints;

        [Tooltip("Delay in seconds before game advances after ball has entered goal.")]
        public float AfterBallEnterGoalDelay = 1.5f;

        private NetworkObject ball;

        private int
        TeamRedSize = 0,
        TeamBlueSize = 0;

        private Dictionary<PlayerRef, PlayerData> PlayerDatas;

        private List<IGamePhase> gamePhases = new List<IGamePhase>
        {
            new WarmupPhase(),
            new MatchPhase(),
            new EndPhase()
        };

        private IGamePhase currentPhase;

        private bool updatePhase = false;

        private struct PendingTeamChange
        {
            public PlayerTeamHandler _handler;
            public Team _team;
        }

        private List<PendingTeamChange> teamChangeQueue = new();

        public void InitializeMatch(NetworkRunner runner)
        {
            // run only on server/host
            if (runner.IsServer)
            {
                // spawn football
                ball = runner.Spawn(BallPrefab, BallSpawnPosition);
                TypeLog(this, "Spawned ball", 1);

                // start first game phase
                updatePhase = true;
                TypeLog(this, "started phase updates", 1);

                TypeLog(this, "init match on host", 1);
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                TypeLogger.TypeLog(this, "detected & destroying duplicate gamemanager instance", 3);
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            NetworkManager.OnSessionStartedEvent += InitializeMatch;
            GameEvents.OnBallEnteredGoal += HandleBallEnteredGoal;
        }

        private void OnDisable()
        {
            NetworkManager.OnSessionStartedEvent -= InitializeMatch;
            GameEvents.OnBallEnteredGoal -= HandleBallEnteredGoal;
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority) return;

            ProcessTeamChanges();

            if (updatePhase) UpdatePhase();
        }

        private void ProcessTeamChanges()
        {
            foreach (var change in teamChangeQueue)
            {
                change._handler.Team = change._team;
                TypeLogger.TypeLog(this, $"Processed team change for player {change._handler.Object.InputAuthority}. new team: {change._team}", 1);
            }
            teamChangeQueue.Clear();
        }

        public NetworkObject GetBall()
        {
            return ball;
        }

        // updates game phases
        private void UpdatePhase()
        {
            // start first phase if no pace 
            if (currentPhase == null)
            {
                BeginPhase(gamePhases[0]);
            }

            if (!currentPhase.IsComplete())
            {
                UpdatePhase(currentPhase);
            }
            else
            {
                EndPhase(currentPhase);
            }
        }

        private void BeginPhase(IGamePhase phase)
        {
            currentPhase = phase;
            currentPhase.OnBegun();
        }

        private void UpdatePhase(IGamePhase phase)
        {
            phase.OnUpdate(Runner.DeltaTime);
        }

        private void EndPhase(IGamePhase phase)
        {
            phase.OnEnd();
            // todo - switch to next phase
        }

        private void HandlePlayerLeft(PlayerData data)
        {
            if (!HasStateAuthority) return;

            // unassign spawn point
            var point = (from spawn in SpawnPoints
                         where spawn._OccupyingPlayer.Ref == data.Ref
                         select spawn).FirstOrDefault();
            if (point != null)
            {
                point._OccupyingPlayer = null;
            }

            switch (data.Team)
            {
                case Team.blue:
                    TeamBlueSize--;
                    break;
                case Team.red:
                    TeamRedSize--;
                    break;
            }

            // update teaming status

            TypeLogger.TypeLog(this, "Unassigned spawn point and team from left player", 1);
        }

        /// <summary>
        /// decides which team to put new player into
        /// </summary>
        private Team DecideTeam()
        {
            if (TeamBlueSize == TeamRedSize)
            {
                return Team.blue; // prefer blue first
            }

            // choose team with less players
            if (TeamBlueSize < TeamRedSize)
            {
                return Team.blue;
            }
            else
            {
                return Team.red;
            }
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            TypeLogger.TypeLog(this, "Player joined", 1);

            if (runner.IsServer)
            {
                // Allow joining only during warmup or before phases are set
                var allowJoin = currentPhase is WarmupPhase || currentPhase == null;
                if (!allowJoin)
                {
                    runner.Disconnect(player);
                }

                // create player 
                var playerData = CreatePlayerData(runner, player);
                AssignTeam(playerData);

                TypeLogger.TypeLog(this, "Assigned team and spawn point for joined player", 1);
            }
        }

        private PlayerData CreatePlayerData(NetworkRunner runner, PlayerRef player)
        {
            var playerObject = runner.Spawn(
                                PlayerPrefab,
                                Vector3.zero,
                                Quaternion.identity,
                                player
                                );

            if (PlayerDatas == null)
                PlayerDatas = new();

            // create data to represent player
            var data = new PlayerData
            {
                Ref = player,
                Object = playerObject,
                Team = Team.none,
            };

            PlayerDatas.Add(player, data);
            return data;
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            // remove player objects
            if (runner.IsServer)
            {
                runner.Despawn(PlayerDatas[player].Object);
                var data = PlayerDatas[player];
                PlayerDatas.Remove(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }

        private void HandleBallEnteredGoal(Team GoalSide)
        {
            if (currentPhase != null && HasStateAuthority)
            {
                currentPhase.OnBallEnteredGoal(GoalSide);
            }
        }

        private void AssignTeam(PlayerData data)
        {
            data.Team = DecideTeam();

            // assign spawn point & place player there
            var viableSpawns = from spawn in SpawnPoints
                               where spawn._Side == data.Team && spawn._OccupyingPlayer == null
                               select spawn;
            var point = viableSpawns.First();
            point._OccupyingPlayer = data;
            data.Object.transform.position = point._Transform.position;
            data.Object.transform.forward = point._Transform.forward;

            // change color based on team
            var color = Colors.TeamColors[data.Team];

            // queue color change - these play around with networked properties so they have to be processed in networkupdate
            var teamChange = new PendingTeamChange()
            {
                _handler = data.Object.GetComponent<PlayerTeamHandler>(),
                _team = data.Team
            };
            teamChangeQueue.Add(teamChange);

            switch (data.Team)
            {
                case Team.blue:
                    TeamBlueSize++;
                    break;
                case Team.red:
                    TeamRedSize++;
                    break;
            }
        }
    }
}

