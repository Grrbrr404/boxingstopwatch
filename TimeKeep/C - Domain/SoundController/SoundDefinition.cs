using System;

namespace TimeKeep.Domain.SoundController
{
    using System.IO;

    using TimeKeep.Domain.RoundManager;

    public class SoundDefinition : ISoundDefinition
    {
        protected string _name;

        protected ManagerPhase _phaseToPlaySound;

        protected TimeSpan _beginPlayback;

        protected TimeSpan _repeatDuration;

        protected string _soundLocation;

        protected bool _isRepeatPlaybackEnabled;

        public string SoundLocation {
            get {
                return _soundLocation;
            }
            set {
                _soundLocation = value;
            }
        }

        public string Name {
            get {
                return _name;
            }
        }

        public ManagerPhase PhaseToPlaySound {
            get {
                return _phaseToPlaySound;
            }
        }

        public TimeSpan BeginPlayback {
            get {
                return _beginPlayback;
            }
        }

        public TimeSpan RepeatDuration {
            get {
                return _repeatDuration;
            }
        }

        public bool IsRepeatPlaybackEnabled {
            get {
                return _isRepeatPlaybackEnabled;
            }
            set {
                _isRepeatPlaybackEnabled = value;
            }
        }

        protected void SetRepeatDuration(TimeSpan repeatDuarion)
        {
            if (repeatDuarion > TimeSpan.Zero) {
                _repeatDuration = repeatDuarion;
                IsRepeatPlaybackEnabled = true;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "{0};{1};{2};{3};{4};{5}",
                Name,
                (int)PhaseToPlaySound,
                BeginPlayback.Ticks,
                RepeatDuration.Ticks,
                IsRepeatPlaybackEnabled,
                SoundLocation
            );
        }

        public static SoundDefinition FromString(string source)
        {
            var values = source.Split(new char[] { ';' });

            var result = new SoundDefinition();
            result._name = values[0];
            result._phaseToPlaySound = (ManagerPhase)(int.Parse(values[1]));
            result._beginPlayback = new TimeSpan(long.Parse(values[2]));
            result._repeatDuration = new TimeSpan(long.Parse(values[3]));
            result._isRepeatPlaybackEnabled = bool.Parse(values[4]);
            result._soundLocation = values[5];

            return result;
        }
    }
}
