using Fusion;

namespace FootBall
{
    public class PlayerController : NetworkBehaviour
    {
        private NetworkCharacterController _characterController;

        public float MoveSpeed = 30f;

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
                _characterController.Move(MoveSpeed * data.direction * Runner.DeltaTime);
            }
        }
    }
}