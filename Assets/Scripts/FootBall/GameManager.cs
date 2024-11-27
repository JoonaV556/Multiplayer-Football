using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

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
    public class GameManager : NetworkBehaviour
    {
        /*
        Responsibilities:
        - Team handling for players
        - Updating game state/phases (warmup, match etc.)
        - Handling ball, goals etc.
        */

        public static GameManager Instance;

        public NetworkObject BallPrefab;

        public Vector3 BallSpawnPosition = Vector3.zero;

        public SpawnPoint[] SpawnPoints;

        private int
        TeamRedSize = 0,
        TeamBlueSize = 0;

        private struct PendingTeamChange
        {
            public PlayerTeamHandler _handler;
            public Team _team;
        }

        private List<PendingTeamChange> teamChangeQueue = new();

        public void InitializeMatch()
        {
            if (HasStateAuthority)
            {
                // spawn football
                Runner.Spawn(BallPrefab, BallSpawnPosition);
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
            NetworkManager.OnAfterPlayerJoined += HandlePlayerJoined;
            NetworkManager.OnAfterPlayerLeft += HandlePlayerLeft;
        }

        private void OnDisable()
        {
            NetworkManager.OnAfterPlayerJoined -= HandlePlayerJoined;
            NetworkManager.OnAfterPlayerLeft -= HandlePlayerLeft;
        }

        public override void FixedUpdateNetwork()
        {
            // process team changes
            foreach (var change in teamChangeQueue)
            {
                change._handler.Team = change._team;
                TypeLogger.TypeLog(this, $"Processed team change for player {change._handler.Object.InputAuthority}. new team: {change._team}", 1);
            }
            teamChangeQueue.Clear();
        }

        private void HandlePlayerJoined(PlayerData data)
        {
            if (!HasStateAuthority) return;

            // assign team
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

            TypeLogger.TypeLog(this, "Assigned team and spawn point for joined player", 1);
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
    }

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

        public void Update();

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

        public void Update()
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
}

