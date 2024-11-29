using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FootBall
{
    /// <summary>
    /// Allows players to join or host a new game session. instantiates gamemanager for host only
    /// </summary>
    public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public string SessionName = "FootballTestSession";

        public GameObject FieldCamera;

        public static event Action<NetworkRunner> OnSessionStartedEvent;

        private NetworkRunner _runner;

        private GameManager gameManager;

        /// <summary>
        /// Starts new network session by hosting or joining existing one
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private async void StartSession(GameMode mode)
        {
            // create runner
            if (gameObject.TryGetComponent(out NetworkRunner runner))
            {
                _runner = runner;
            }
            else
            {
                _runner = gameObject.AddComponent<NetworkRunner>();
                TypeLogger.TypeLog(this, "runner not found. created new one instead", 2);
            }

            _runner.ProvideInput = true;

            _runner.AddCallbacks(GameManager.Instance);

            // create scene info from current scene
            var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            var info = new NetworkSceneInfo();
            if (scene.IsValid)
            {
                info.AddSceneRef(scene, LoadSceneMode.Additive);
            }

            // Start or join session
            var args = new StartGameArgs()
            {
                GameMode = mode,
                SessionName = SessionName,
                Scene = scene,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            };

            if (args.GameMode == GameMode.Host)
            {
                args.PlayerCount = 4; // limit to 4 players
            }

            await _runner.StartGame(args);

            OnSessionStarted();

            TypeLogger.TypeLog(this, @$"
            started game with mode {mode},
            session name FootballTestSession, 
            ", 1);
        }

        private void OnSessionStarted()
        {
            OnSessionStartedEvent?.Invoke(_runner);
        }

        private void OnGUI()
        {
            if (_runner == null)
            {
                if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
                {
                    StartSession(GameMode.Host);
                }
                if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
                {
                    StartSession(GameMode.Client);
                }
            }
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            TypeLogger.TypeLog(this, "Connected to server", 1);
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            // Disable overview camera when player spawns - player will see game through first person
            if (runner.LocalPlayer == player)
            {
                FieldCamera.SetActive(false);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (_runner.IsServer)
            {
                _runner.RemoveCallbacks(
                   new INetworkRunnerCallbacks[] {
                        gameManager
                   }
               );
            }
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }
    }
}

