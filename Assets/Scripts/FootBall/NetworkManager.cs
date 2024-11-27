using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FootBall
{
    /// <summary>
    /// Allows clients and host to join or host a new game session. instantiates gamemanager for host 
    /// </summary>
    public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public string SessionName = "FootballTestSession";

        public NetworkObject PlayerPrefab;

        public NetworkObject GameManager;

        public GameObject FieldCamera;

        private NetworkRunner _runner;

        private Dictionary<PlayerRef, PlayerData> PlayerDatas; // tracks data related to each player

        public static event Action<PlayerData> OnAfterPlayerJoined, OnAfterPlayerLeft;

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
                SessionName = "FootballTestSession",
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
            if (_runner.IsServer)
            {
                // Create football game manager on host 
                NetworkObject gameManagerObj = _runner.Spawn(GameManager, Vector3.zero, Quaternion.identity, _runner.LocalPlayer);

                // Run game initialization
                gameManagerObj.GetComponent<GameManager>().InitializeMatch();
            }
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
            TypeLogger.TypeLog(this, "Player joined", 1);

            // Spawn Players
            if (runner.IsServer)
            {
                var playerObject = runner.Spawn(
                    PlayerPrefab,
                    Vector3.zero,
                    Quaternion.identity,
                    player
                    );

                if (PlayerDatas == null)
                    PlayerDatas = new();

                var data = new PlayerData
                {
                    Ref = player,
                    Object = playerObject,
                    Team = Team.none, // teaming is done by gamemanager, which runs on host only
                };

                PlayerDatas.Add(player, data);

                OnAfterPlayerJoined?.Invoke(data);
            }

            // Disable overview camera when player spawns - player will see game through first person
            if (runner.LocalPlayer == player)
            {
                FieldCamera.SetActive(false);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            // remove player objects
            if (runner.IsServer)
            {
                runner.Despawn(PlayerDatas[player].Object);
                var data = PlayerDatas[player];
                PlayerDatas.Remove(player);
                OnAfterPlayerLeft?.Invoke(data);
            }
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
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }
    }
}

