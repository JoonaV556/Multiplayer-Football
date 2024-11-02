using UnityEngine;
using Fusion;

namespace FootBall
{
    public class PlayerController : NetworkBehaviour
    {
        public GameObject FpCamera;
        public GameObject BodyVisual;

        public override void Spawned()
        {
            // prepare player clientside
            if (HasInputAuthority)
            {
                FpCamera.SetActive(true); // Activate first person camera
                BodyVisual.SetActive(false);
            }
        }
    }
}