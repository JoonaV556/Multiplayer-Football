using UnityEngine;

namespace FootBall
{
    public class GoalTrigger : MonoBehaviour
    {
        public string BallTag;

        public Team GoalSide = Team.none; // which side does goal belong to

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(BallTag))
            {
                TypeLogger.TypeLog(this, $"ball entered {GoalSide} team goal!", 1);
                GameEvents.OnBallEnteredGoal?.Invoke(GoalSide);
            }
        }
    }
}

