using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;
using Fusion.Sockets;
using System.Collections.Generic;
using System;

namespace FootBall
{
    public class InputManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public InputActionAsset inputActions;

        [Range(0f, 10f)]
        public float LookVerticalSensitivityMultiplier = 1f;
        [Range(0f, 10f)]
        public float LookHorizontalSensitivityMultiplier = 1f;

        InputAction moveAction;
        InputAction lookAction;

        public static InputData Data = new InputData(); // other objects can read this

        private void OnEnable()
        {
            inputActions.Enable();

            // find actions
            moveAction = inputActions.FindAction("Move", true);
            lookAction = inputActions.FindAction("Look", true);
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            // send input over to server
            Data.MoveInput = moveAction.ReadValue<Vector2>();
            var rawLookInput = lookAction.ReadValue<Vector2>();
            // apply sensitivity multipliers to look input
            Data.LookInput = new Vector2(
                rawLookInput.x * LookHorizontalSensitivityMultiplier,
                rawLookInput.y * LookVerticalSensitivityMultiplier
            );

            input.Set(Data);
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
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
    }
}

