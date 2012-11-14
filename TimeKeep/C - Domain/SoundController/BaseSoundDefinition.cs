using System;

namespace TimeKeep.Domain.SoundController
{
    using System.IO;

    using TimeKeep.Domain.RoundManager;

    public abstract class BaseSoundDefinition : ISoundDefinition
    {
        protected string _name;

        protected ManagerPhase _phaseToPlaySound;

        protected TimeSpan _playSoundAtSecond;

        private TimeSpan _repeatDuration;

        private string _soundSource;

        private bool isRepeatPlaybackEnabled;

        public string SoundSource {
            get {
                return _soundSource;
            }
            set {
                _soundSource = value;
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
                return _playSoundAtSecond;
            }
        }

        public TimeSpan RepeatDuration {
            get {
                return _repeatDuration;
            }
        }

        public bool IsRepeatPlaybackEnabled {
            get {
                return isRepeatPlaybackEnabled;
            }
            set {
                isRepeatPlaybackEnabled = value;
            }
        }

        protected void SetRepeatDuration(TimeSpan repeatDuarion)
        {
            if (repeatDuarion > TimeSpan.Zero) {
                _repeatDuration = repeatDuarion;
                IsRepeatPlaybackEnabled = true;
            }
        }
    }
}
