using System;
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
        public NetworkObject BallPrefab;

        public Vector3 BallSpawnPosition = Vector3.zero;

        public SpawnPoint[] SpawnPoints;

        private int
        TeamRedSize = 0,
        TeamBlueSize = 0;

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

            // place on team spawn position

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

        private void HandlePlayerLeft(PlayerData data)
        {
            if (!HasStateAuthority) return;

            // unassign spawn point

            // update teaming status
        }
    }
}

