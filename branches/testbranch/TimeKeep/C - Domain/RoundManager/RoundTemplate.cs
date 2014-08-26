using System;

namespace TimeKeep.Domain.RoundManager
{
    [Serializable]
    public class RoundTemplate
    {
        public IRoundDefinition Definition { get; set; }

        public string Round
        {
            get
            {
                return TimeSpan.FromSeconds(Definition.GetRoundTimeInSeconds()).ToString(@"mm\:ss");
            }
        }

        public string Pause
        {
            get
            {
                return TimeSpan.FromSeconds(Definition.GetPauseTimeInSeconds()).ToString(@"mm\:ss");
            }
        }

        public int MaxRounds
        {
            get 
            {
                return Definition.GetMaxRounds();
            }
        }

        public RoundTemplate(IRoundDefinition definition)
        {
            Definition = definition;
        }
    }
}
