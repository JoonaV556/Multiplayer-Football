using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FootBall
{
    public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public GameObject PlayerPrefab;

        private NetworkRunner runner; // heart of photon

        private Dictionary<PlayerRef, NetworkObject> PlayerObjects = new();

        Vector3 lastSpawnPoint = Vector3.zero;

        private async void StartGame(GameMode mode)
        {
            // create runner
            runner = gameObject.AddComponent<NetworkRunner>();
            runner.ProvideInput = true;

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

            await runner.StartGame(
                args
            );

            TypeLogger.TypeLog(this, @$"
            started game with mode {mode},
            session name FootballTestSession, 
            ", 1);
        }

        private void OnGUI()
        {
            if (runner == null)
            {
                if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
                {
                    StartGame(GameMode.Host);
                }
                if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
                {
                    StartGame(GameMode.Client);
                }
            }
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            // Handle logic when connected to server
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            // Handle logic when connection fails
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            // Handle logic for connection requests
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            // Handle custom authentication responses
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            // Handle logic when disconnected from server
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            // Handle host migration logic
        }

        private NetworkInputData inputData = new();
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            inputData.direction = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
                inputData.direction += Vector3.forward;

            if (Input.GetKey(KeyCode.S))
                inputData.direction += Vector3.back;

            if (Input.GetKey(KeyCode.A))
                inputData.direction += Vector3.left;

            if (Input.GetKey(KeyCode.D))
                inputData.direction += Vector3.right;

            input.Set(inputData);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            // Handle missing input logic
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            // Handle logic when object enters Area of Interest (AOI)
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            // Handle logic when object exits Area of Interest (AOI)
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                TypeLogger.TypeLog(this, @$"Player with id {player.PlayerId} joined game", 1);

                // create object for player
                // get spawn position
                Vector3 spawnPos = lastSpawnPoint + new Vector3(2f, 0, 0);
                lastSpawnPoint = spawnPos;
                // spawn
                NetworkObject networkPlayerObject = runner.Spawn(PlayerPrefab, spawnPos, Quaternion.identity, player);
                // save for further handling
                PlayerObjects.Add(player, networkPlayerObject);

                // RPC_OnPlayerSpawned(player, networkPlayerObject);
            }
        }

        // [Rpc(RpcSources.StateAuthority, RpcTargets.All, RpcHostMode.SourceIsServer)]
        // public void RPC_OnPlayerSpawned(PlayerRef player, NetworkObject playerObject)
        // {
        //     // activate camera on player object if its our player
        //     if (playerObject.HasInputAuthority) // player == runner.localPlayer()
        //     {
        //         playerObject.gameObject.GetComponentInChildren<Camera>().gameObject.SetActive(true);
        //     }
        // }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                TypeLogger.TypeLog(this, @$"Player with id {player.PlayerId} left game", 1);

                // remove player object
                if (PlayerObjects.TryGetValue(player, out NetworkObject networkObject))
                {
                    runner.Despawn(networkObject);
                    PlayerObjects.Remove(player);
                    TypeLogger.TypeLog(this, @$"Removed left players object", 1);
                }
            }
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
            // Handle reliable data progress
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            // Handle reliable data received
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            // Handle logic when scene load is done
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            // Handle logic when scene load starts
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            // Handle session list updates
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            // Handle logic when runner shuts down
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            // Handle user simulation messages
        }
    }
}