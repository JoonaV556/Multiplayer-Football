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
        private NetworkRunner runner; // heart of photon

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

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            // Handle input logic
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
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            // Handle logic when player leaves
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