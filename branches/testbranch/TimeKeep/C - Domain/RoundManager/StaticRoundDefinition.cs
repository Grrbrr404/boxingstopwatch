using System;

namespace TimeKeep.Domain.RoundManager
{
    class StaticRoundDefinition : IRoundDefinition
    {
        public int RoundInSeconds { get; set; }

        public int PauseInSeconds { get; set; }

        public int MaxRounds { get; set; }


        #region Implementation of IRoundDefinition
        
        /// <summary>
        /// Determines how many rounds should be executed. Return 0 for infinite rounds.
        /// </summary>
        /// <returns></returns>
        public int GetMaxRounds()
        {
            return MaxRounds;
        }

        /// <summary>
        /// Determines the length of one round in seconds.
        /// </summary>
        /// <returns></returns>
        public int GetRoundTimeInSeconds()
        {
            return RoundInSeconds;
        }

        /// <summary>
        /// Determines the length of a pause between two rounds.
        /// </summary>
        /// <returns></returns>
        public int GetPauseTimeInSeconds()
        {
            return PauseInSeconds;
        }

        #endregion
    }
}
