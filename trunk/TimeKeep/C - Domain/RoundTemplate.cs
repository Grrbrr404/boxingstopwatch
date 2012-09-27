using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeKeep.Domain
{
    using TimeKeep.Domain.Interfaces;

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

        public int MaxRounds()
        {
            return Definition.GetMaxRounds();
        }

        public RoundTemplate(IRoundDefinition definition)
        {
            Definition = definition;
        }
    }
}
