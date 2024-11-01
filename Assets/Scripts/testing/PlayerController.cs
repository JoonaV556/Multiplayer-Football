using Fusion;
using UnityEngine;

namespace FusionTutorial
{
    public class PlayerController : NetworkBehaviour
    {
        public NetworkObject BallPrefab;
        public float BallFireCooldown = 5f;

        private NetworkCharacterController _characterController;

        [Networked] private TickTimer delay { get; set; }

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

                // Spawn ball if we are host & firing is not on cooldown
                if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
                {
                    if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                    {
                        delay = TickTimer.CreateFromSeconds(Runner, BallFireCooldown);
                        Runner.Spawn(
                            BallPrefab,
                            new Vector3(transform.position.x, transform.position.y + 1, transform.position.z) + data.direction,
                            Quaternion.LookRotation(data.direction),
                            Object.InputAuthority,
                                (runner, o) =>
                                {
                                    o.GetComponent<NetworkMover>().Init();
                                }
                            );
                    }
                }
            }
        }
    }
}