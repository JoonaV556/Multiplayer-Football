using Fusion;
using UnityEngine;

namespace FootBall
{
    /// <summary>
    /// Manages football game-related logic. Exists only on host.
    /// </summary>
    public class GameManager : NetworkBehaviour
    {
        public NetworkObject BallPrefab;

        public Vector3 BallSpawnPosition = Vector3.zero;

        public void InitializeMatch()
        {
            if (HasStateAuthority)
            {
                // spawn football
                Runner.Spawn(BallPrefab, BallSpawnPosition);

                // Spawn another for testing
                Runner.Spawn(BallPrefab, BallSpawnPosition + new Vector3(0f, 1f, 0f));
            }
        }
    }
}

