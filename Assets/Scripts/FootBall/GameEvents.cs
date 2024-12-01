using System;

namespace FootBall
{
    public static class GameEvents
    {
        /// <summary>
        /// fired when ball enters a goal. Team argument for which side goal ball entered
        /// </summary>
        public static Action<Team> OnBallEnteredGoal;
    }
}
