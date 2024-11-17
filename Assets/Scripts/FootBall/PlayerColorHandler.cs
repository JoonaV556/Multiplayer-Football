using Fusion;
using UnityEngine;

namespace FootBall
{
    /// <summary>
    /// Handles team-change related behavior for individual player objects
    /// </summary>
    public class PlayerTeamHandler : NetworkBehaviour
    {
        public MeshRenderer[] ColorRenderers;
        [Networked, HideInInspector] public Team Team { get; set; }

        private Team localTeam = Team.none;

        public override void FixedUpdateNetwork()
        {
            if (localTeam != Team)
            {
                UpdateColor(Team);
            }
        }

        private void UpdateColor(Team team)
        {
            localTeam = team;
            var color = Colors.TeamColors[team];
            foreach (var renderer in ColorRenderers)
            {
                renderer.material.color = color;
            }
            TypeLogger.TypeLog(this, "changed players color", 1);
        }
    }
}
