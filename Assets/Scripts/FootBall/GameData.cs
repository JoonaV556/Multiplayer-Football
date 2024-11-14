using Fusion;

namespace FootBall
{
    public enum Team
    {
        none,
        red,
        blue
    }

    public class PlayerData
    {
        public PlayerRef Ref;
        public NetworkObject Object;
        public Team Team;

        public PlayerData() { }
    }
}