using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

namespace FootBall
{
    public class InputManager : NetworkBehaviour
    {
        public InputActionAsset inputActions;

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

        public override void FixedUpdateNetwork()
        {
            // update data each tick
            Data.MoveInput = moveAction.ReadValue<Vector2>();
            Data.LookInput = lookAction.ReadValue<Vector2>();
            TypeLogger.TypeLog(this, $"move vector {Data.MoveInput}", 1);
            TypeLogger.TypeLog(this, $"look delta {Data.LookInput}", 1);
        }
    }
}

