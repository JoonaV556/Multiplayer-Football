using System;
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

        [Networked] public Team Team { get; set; }

        public static event Action<Team> OnLocalPlayerTeamChanged;

        public Team localTeam = Team.none;

        public override void FixedUpdateNetwork()
        {
            if (localTeam != Team)
            {
                UpdateTeam(Team);
            }
        }

        private void Update()
        {
            if (localTeam != Team)
            {
                UpdateTeam(Team);
            }
        }

        private void UpdateTeam(Team team)
        {
            var lastTeam = localTeam;

            localTeam = team;
            var color = Colors.TeamColors[team];
            foreach (var renderer in ColorRenderers)
            {
                renderer.material.color = color;
            }
            TypeLogger.TypeLog(this, $"updated player {Object.InputAuthority} team. last team: {lastTeam}, new team: {team}", 1);

            if (HasInputAuthority)
            {
                TypeLogger.TypeLog(this, "Our player team changed", 1);
                OnLocalPlayerTeamChanged?.Invoke(team);
            }
            else
            {
                TypeLogger.TypeLog(this, "Other player's team changed", 1);
            }
        }
    }
}
