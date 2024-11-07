using Fusion;
using UnityEngine;

namespace FootBall
{
    /// <summary>
    /// Manages football game-related logic.
    /// </summary>
    public class GameManager : NetworkBehaviour
    {
        public NetworkObject BallPrefab;

        public Vector3 BallSpawnPosition = Vector3.zero;

        public void InitializeMatch()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (HasStateAuthority)
            {
                // spawn football
                Runner.Spawn(BallPrefab, BallSpawnPosition);

                // Spawn another for testing
                //Runner.Spawn(BallPrefab, BallSpawnPosition + new Vector3(0f, 1f, 0f));
            }
        }
    }
}

