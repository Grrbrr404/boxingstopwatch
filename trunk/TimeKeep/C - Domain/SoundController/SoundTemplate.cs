using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeKeep.Domain.RoundManager;
using System.IO;

namespace TimeKeep.Domain.SoundController
{
    public class SoundTemplate
    {

        public bool IsActive { get; set; }
        public SoundDefinition Definition { get; set; }

        public string Time
        {
            get
            {
                // We dont want to have negative values here
                return string.Format(
                  "{0:00}:{1:00}:{2:00}",
                  Definition.BeginPlayback.Minutes,
                  Definition.BeginPlayback.Seconds,
                  Definition.BeginPlayback.Milliseconds / 10.0);
            }
        }
        
        public string File
        {
            get
            {
                return Path.GetFileName(Definition.SoundLocation);
            }
        }


        public override string ToString()
        {
            return string.Format("{0};{1}", IsActive, Definition.ToString());
        }

        public static SoundTemplate FromString(string source)
        {
            var values = source.Split(new char[] { ';' });
            var result = new SoundTemplate();
            result.IsActive = bool.Parse(values[0]);
            
            // values[0].length + 1 because there is a ; behind the IsActive indicator
            var subsource = source.Substring(values[0].Length + 1);
            result.Definition = SoundDefinition.FromString(subsource);
            return result;
        }
    }
}
