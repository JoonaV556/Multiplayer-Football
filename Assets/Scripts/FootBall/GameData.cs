using UnityEngine;
using Fusion;
using System.Collections.Generic;

namespace FootBall
{
    public enum Team
    {
        none,
        red,
        blue
    }

    public static class Colors
    {
        public static Color TeamRedColor = new Color(1f, 0.3040164f, 0.209f, 1f);
        public static Color TeamBlueColor = new Color(0.2396226f, 0.6590682f, 1f, 1f);

        public static Dictionary<Team, Color> TeamColors = new Dictionary<Team, Color>
        {
            {Team.none, Color.black},
            {Team.red, TeamRedColor},
            {Team.blue, TeamBlueColor},
        };
    }

    public class PlayerData
    {
        public PlayerRef Ref;
        public NetworkObject Object;
        public Team Team;

        public PlayerData() { }
    }
}