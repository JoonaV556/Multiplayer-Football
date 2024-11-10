using UnityEngine;
using Fusion;
using static TypeLogger;

namespace FootBall
{
    public class PlayerController : NetworkBehaviour
    {
        public GameObject FpCamera;
        public GameObject BodyVisual;

        [Range(0.1f, 89f)]
        public float CameraRotMaxY = 89f;
        [Range(-0.1f, -89f)]
        public float CameraRotMinY = -89f;

        public float MoveForce = 2f;

        Rigidbody _rb;

        private float lookYRot = 0f;

        public override void Spawned()
        {
            TypeLog(this, "we spawned", 1);
            this._rb = gameObject.GetComponent<Rigidbody>();

            // prepare player object clientside
            if (HasInputAuthority)
            {
                TypeLog(this, "we have input authority", 1);
                this.FpCamera.SetActive(true); // If this player is us, make us see through the characters first person camera
            }
            else
            {
                this.BodyVisual.SetActive(true);
                TypeLog(this, "we activated body visual", 1); // If this is not our player, make it visible to us
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (_rb == null) return;

            // get input from the player who has input authority over this controller
            if (GetInput(out InputData data))
            {
                // turn player body left and right with look input
                var bodyRot = transform.rotation.eulerAngles;
                var newRot = bodyRot + new Vector3(
                    0f,
                    data.LookInput.x * Runner.DeltaTime,
                    0f
                    );
                _rb.MoveRotation(Quaternion.Euler(newRot));

                // move player 
                _rb.AddForce(
                    transform.TransformDirection(
                        data.MoveInput.x,
                        0f,
                        data.MoveInput.y
                    ).normalized * MoveForce,
                    ForceMode.Force);

                // turn player camera up and down
                var change = -data.LookInput.y * Runner.DeltaTime;
                lookYRot = Mathf.Clamp(lookYRot + change, CameraRotMinY, CameraRotMaxY);
                FpCamera.transform.localRotation = Quaternion.Euler(lookYRot, 0f, 0f);
            }
        }
    }
}