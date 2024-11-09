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

        public override void Spawned()
        {
            TypeLog(this, "we spawned", 1);
            // prepare player clientside
            if (HasInputAuthority)
            {
                TypeLog(this, "we have input authority", 1);
                this.FpCamera.SetActive(true); // Activate first person camera
                this._rb = GetComponent<Rigidbody>();
            }
            else
            {
                this.BodyVisual.SetActive(true);
                TypeLog(this, "we activated body visual", 1);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasInputAuthority) return;

            // turn player body left and right with look input
            var rot = transform.rotation.eulerAngles;
            var newRot = rot + new Vector3(
                0f,
                (InputManager.Data.LookInput.x * Runner.DeltaTime),
                0f
                );
            _rb.MoveRotation(Quaternion.Euler(newRot));

            // move player 
            _rb.AddForce(
                transform.TransformDirection(
                    InputManager.Data.MoveInput.x,
                    0f,
                    InputManager.Data.MoveInput.y
                ).normalized * MoveForce,
                ForceMode.Force);
        }

        private float rot = 0f;
        private void Update()
        {
            if (!HasInputAuthority) return;

            var change = -InputManager.Data.LookInput.y * Time.deltaTime;
            rot = Mathf.Clamp(rot + change, CameraRotMinY, CameraRotMaxY);

            // turn player camera up and down
            FpCamera.transform.localRotation = Quaternion.Euler(rot, 0f, 0f);
        }
    }
}