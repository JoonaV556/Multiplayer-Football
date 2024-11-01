using Fusion;
using UnityEngine;

namespace FusionTutorial
{
    public struct NetworkInputData : INetworkInput
    {
        public const byte MOUSEBUTTON0 = 1;

        public Vector3 direction;
        public NetworkButtons buttons;
    }
}
