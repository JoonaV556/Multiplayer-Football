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

        public static InputData Data = new InputData(); // other objects can read this

        [Range(0f, 10f)]
        public float LookVerticalSensitivityMultiplier = 1f;
        [Range(0f, 10f)]
        public float LookHorizontalSensitivityMultiplier = 1f;

        private float
        maxSensMultiplier = 10f;

        InputAction
        toggleCursorAction,
        moveAction,
        lookAction,
        toggleMenuAction,
        jumpAction,
        directionalJumpAction;

        public void SetHorizontalLookSensitivityMultiplier(float alpha)
        {
            var value = Mathf.Clamp(alpha, 0f, 1f);
            LookHorizontalSensitivityMultiplier = value * maxSensMultiplier;
        }
        public void SetVerticalLookSensitivityMultiplier(float alpha)
        {
            var value = Mathf.Clamp(alpha, 0f, 1f);
            LookVerticalSensitivityMultiplier = value * maxSensMultiplier;
        }

        private void OnEnable()
        {
            inputActions.Enable();

            // get actions from asset
            moveAction = inputActions.FindAction("Move", true);
            lookAction = inputActions.FindAction("Look", true);
            toggleMenuAction = inputActions.FindAction("ToggleMenu", true);
            toggleCursorAction = inputActions.FindAction("ToggleCursorLock", true);
            jumpAction = inputActions.FindAction("Jump", true);
            directionalJumpAction = inputActions.FindAction("DirectionalJump", true);

            Cursor.lockState = CursorLockMode.Locked;
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
            Data.JumpTriggered = jumpAction.WasPressedThisFrame();
            Data.DirectionalJumpTriggered = directionalJumpAction.WasPressedThisFrame();

            input.Set(Data);
        }

        private void Update()
        {
            Data.ToggleMenuTriggered = false;
            // no reason to send menu key data over network so we update it clientside
            if (toggleMenuAction.WasPressedThisFrame())
            {
                Data.ToggleMenuTriggered = true;
            }
            if (toggleCursorAction.WasPressedThisFrame())
            {
                switch (Cursor.lockState)
                {
                    case CursorLockMode.Locked:
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        break;
                    case CursorLockMode.None:
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        break;
                    default:
                        break;
                }
            }
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

