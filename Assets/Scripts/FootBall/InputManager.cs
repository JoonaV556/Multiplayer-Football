using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

namespace FootBall
{
    public class InputManager : NetworkBehaviour
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

        private void Update()
        {
            // Update input
            Data.MoveInput = moveAction.ReadValue<Vector2>();
            var rawLookInput = lookAction.ReadValue<Vector2>();
            // apply sensitivity multipliers to look input
            Data.LookInput = new Vector2(
                rawLookInput.x * LookHorizontalSensitivityMultiplier,
                rawLookInput.y * LookVerticalSensitivityMultiplier
            );


            // TypeLogger.TypeLog(this, $"move vector {Data.MoveInput}", 1);
            // TypeLogger.TypeLog(this, $"look delta {Data.LookInput}", 1);
        }
    }
}

