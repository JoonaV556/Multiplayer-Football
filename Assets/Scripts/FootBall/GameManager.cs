using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
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
    /// Manages football game-related logic. All logic runs mainly on host/server
    /// </summary>
    public class GameManager : NetworkBehaviour
    {
        private struct PendingTeamChange
        {
            public PlayerTeamHandler _handler;
            public Team _team;
        }

        public NetworkObject BallPrefab;

        public Vector3 BallSpawnPosition = Vector3.zero;

        public SpawnPoint[] SpawnPoints;

        private int
        TeamRedSize = 0,
        TeamBlueSize = 0;

        private List<PendingTeamChange> teamChangeQueue = new();

        public void InitializeMatch()
        {
            if (HasStateAuthority)
            {
                // spawn football
                Runner.Spawn(BallPrefab, BallSpawnPosition);
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
}

