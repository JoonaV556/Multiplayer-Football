using Fusion;
using UnityEngine;

namespace FootBall
{
    public struct InputData : INetworkInput
    {
        public Vector2 MoveInput;
        public Vector2 LookInput; // delta

        public bool ToggleMenuTriggered;
        public bool JumpTriggered;
        public bool DirectionalJumpActive;
    }

}

