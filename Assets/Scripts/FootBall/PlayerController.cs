using UnityEngine;
using Fusion;
using static TypeLogger;

namespace FootBall
{
    /// <summary>
    /// Controls player movement & camera & body visibility
    /// </summary>
    public class PlayerController : NetworkBehaviour
    {
        public GameObject CameraPrefab;
        public GameObject CameraFollow;
        public GameObject BodyVisual;

        [Range(0.1f, 89f)]
        public float CameraRotMaxY = 89f;
        [Range(-0.1f, -89f)]
        public float CameraRotMinY = -89f;

        public float MoveForce = 2f;

        private float lookYRot = 0f;

        Rigidbody _rb;

        public override void Spawned()
        {
            TypeLog(this, "we spawned", 1);
            this._rb = gameObject.GetComponent<Rigidbody>();

            // prepare player object clientside
            if (HasInputAuthority)
            {
                // This player is ours - init stuff on it for us
                TypeLog(this, "we have input authority", 1);
                // We need a separate camera object because of jitter caused by rigidbody 
                // separate object allows us to smooth camera movement
                this.CameraFollow.SetActive(true);
                var _cameraObj = Instantiate(CameraPrefab);
                // init camera controller 
                _cameraObj.GetComponent<CameraController>().Init(CameraFollow.transform);
            }
            else
            {
                // This player is not ours - make it visible to us
                this.BodyVisual.SetActive(true);
                TypeLog(this, "we activated body visual", 1);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (_rb == null) return;

            // get input from the player who has input authority over this controller
            if (GetInput(out InputData data))
            {
                // turn player body left and right with look input
                var newRot = new Vector3(
                    0f,
                    data.LookInput.x * Runner.DeltaTime,
                    0f
                    );
                transform.Rotate(newRot);

                // TypeLog(this, @$"Horizontal look info
                // data: {data.LookInput.x},
                // runner delta {Runner.DeltaTime},
                // applied horizontal rot of {newRot}", 1);

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
                CameraFollow.transform.localRotation = Quaternion.Euler(lookYRot, 0f, 0f);

                // TypeLog(this, @$"Vertical look info
                // data: {-data.LookInput.y},
                // runner delta {Runner.DeltaTime},
                // new y rot {lookYRot}", 1);
            }
        }
    }
}