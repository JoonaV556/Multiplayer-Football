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

        [Networked] public Team Team { get; set; } = Team.none;

        public static event Action<Team> OnLocalPlayerTeamChanged;

        private Team localTeam = Team.none;

        private ChangeDetector _changeDetector;

        public override void Spawned()
        {
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        public override void FixedUpdateNetwork()
        {
            if (localTeam != Team)
            {
                UpdateTeam(Team);
            }

            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(Team):
                        TypeLogger.TypeLog(this, "Detected team change w changedetector", 1);
                        break;
                }
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
            TypeLogger.TypeLog(this, $"updated player {gameObject.name} team. last team: {lastTeam}, new team: {team}", 1);

            if (HasInputAuthority)
            {
                OnLocalPlayerTeamChanged?.Invoke(team);
            }
        }
    }
}
