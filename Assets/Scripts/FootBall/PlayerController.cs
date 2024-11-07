using UnityEngine;
using Fusion;

namespace FootBall
{
    public class PlayerController : NetworkBehaviour
    {
        public GameObject FpCamera;
        public GameObject BodyVisual;

        private Rigidbody rigidbody;

        public override void Spawned()
        {
            // prepare player clientside
            if (HasInputAuthority)
            {
                FpCamera.SetActive(true); // Activate first person camera
                BodyVisual.SetActive(false);
            }

            rigidbody = GetComponent<Rigidbody>();
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
            rigidbody.MoveRotation(Quaternion.Euler(newRot));
        }
        public Vector3 rot;
        private void Update()
        {
            if (!HasInputAuthority) return;
            // Limit camera vertical angle
            rot = FpCamera.transform.localEulerAngles;
            var change = (-InputManager.Data.LookInput.y * Time.deltaTime);

            if (
                rot.x > 0.01f
                &&
                rot.x < 89.99f
                &&
                (rot.x + change) > 89.99f
                )
            {
                rot = new Vector3(
                    Mathf.Clamp(rot.x + change, 0.01f, 89.99f),
                    0f,
                    0f
                );
                FpCamera.transform.localEulerAngles = rot;
                TypeLogger.TypeLog(this, "limiting down", 1);
                return;
            }

            if (
                rot.x < 359.99f
                &&
                rot.x > 270.01f
                &&
                (rot.x + change) < 270.01f
                )
            {
                rot = new Vector3(
                    Mathf.Clamp(rot.x + change, 270.01f, 359.99f),
                    0f,
                    0f
                );
                FpCamera.transform.localEulerAngles = rot;
                TypeLogger.TypeLog(this, "limiting up", 1);
                return;
            }

            // turn player camera up and down
            FpCamera.transform.Rotate(
                -InputManager.Data.LookInput.y * Time.deltaTime,
                0f,
                0f
                );
        }
    }
}