using Fusion;
using UnityEngine;

namespace FootBall
{
    public class PlayerColorHandler : NetworkBehaviour
    {
        public MeshRenderer[] ColorRenderers;
        [Networked, HideInInspector] public Team Color { get; set; }

        private Team localColor = Team.none;

        public override void FixedUpdateNetwork()
        {
            if (localColor != Color)
            {
                UpdateColor(Color);
            }
        }

        private void UpdateColor(Team Color)
        {
            localColor = Color;
            var color = new Color();
            switch (Color)
            {
                case Team.red:
                    color = Colors.TeamRedColor;
                    break;
                case Team.blue:
                    color = Colors.TeamBlueColor;
                    break;
                default:
                    color = UnityEngine.Color.black;
                    break;
            }

            foreach (var renderer in ColorRenderers)
            {
                renderer.material.color = color;
            }
        }
    }
}
