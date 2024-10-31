using Fusion;
using UnityEngine;

namespace FootBall
{
    public class PlayerController : NetworkBehaviour
    {
        private NetworkCharacterController _characterController;

        private void Awake()
        {
            _characterController = GetComponent<NetworkCharacterController>();
        }

        // update loop for networked objects
        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                data.direction.Normalize();
                _characterController.Move(data.direction * Runner.DeltaTime);
            }
        }
    }
}