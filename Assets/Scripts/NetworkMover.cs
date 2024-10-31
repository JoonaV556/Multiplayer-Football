using Fusion;

namespace FootBall
{
    public class NetworkMover : NetworkBehaviour
    {
        public float speed = 5f;
        public float lifeTime = 5f;

        [Networked]
        private TickTimer lifeTimer { get; set; }

        public void Init()
        {
            lifeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
        }

        // sync movement to network framerate
        // instead of Update()       -> FixedUpdateNetwork()
        // instead of Time.deltaTime -> Runner.DeltaTime 
        public override void FixedUpdateNetwork()
        {
            if (lifeTimer.Expired(Runner))
                Runner.Despawn(Object);
            else
                transform.position += speed * transform.forward * Runner.DeltaTime;
        }
    }
}

