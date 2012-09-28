using System;

namespace TimeKeep.Domain
{
    using System.Threading;

    using Caliburn.Micro;

    using TimeKeep.Domain.Interfaces;
    using TimeKeep.Foundation.Threading;
    using TimeKeep.Foundation.Threading.Interfaces;

    /// <summary>
    /// Timer class with start / stop functionality.
    /// </summary>
    public class RoundManager : PropertyChangedBase
    {

        private const int TIMER_INTERVAL = 100;

        #region Variables

        /// <summary>
        /// Round definition for this RoundManger instance.
        /// </summary>
        private IRoundDefinition _roundDefinition;

        /// <summary>
        /// Rounds finished
        /// </summary>
        private int _countFinishedRounds = 0;

        private int _currentRound = 0;

        /// <summary>
        /// Timer thread. Used to count down rounds / pause
        /// </summary>
        private BackgroundAction _timerAction;

        private ManagerPhase _phase = ManagerPhase.Round;

        private TimeSpan _remainingTimeOfPhase = TimeSpan.Zero;

        #endregion

        #region Constructor
        public RoundManager(IRoundDefinition roundDef)
        {
            SetDefinition(roundDef);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Rounds finished
        /// </summary>
        public int CountFinishedRounds
        {
            get
            {
                return _countFinishedRounds;
            }
            private set
            {
                _countFinishedRounds = value;
                NotifyOfPropertyChange(() => CountFinishedRounds);
            }
        }

        public int CurrentRound
        {
            get
            {
                return _currentRound;
            }
            private set
            {
                _currentRound = value;
                NotifyOfPropertyChange(() => CurrentRound);
            }
        }

        /// <summary>
        /// Indicates if this RoundManager does use max rounds or not
        /// </summary>
        public int MaxRounds
        {
            get
            {
                return RoundDefinition.GetMaxRounds();
            }
        }

        /// <summary>
        /// Indicates if the internal timer thread is currently running or not
        /// </summary>
        public bool IsRunning
        {
            get
            {
                if (_timerAction == null) return false;

                return _timerAction.State == BackgroundProcessState.Running;
            }
        }

        public string Time
        {
            get
            {
                return string.Format(
                  "{0:00}:{1:00}:{2:00}",
                  RemainingTime.Minutes,
                  RemainingTime.Seconds,
                  RemainingTime.Milliseconds / 10.0);
            }
        }

        /// <summary>
        /// Round definition for this RoundManger instance.
        /// </summary>
        public IRoundDefinition RoundDefinition
        {
            get
            {
                return _roundDefinition;
            }
        }

        public TimeSpan RemainingTime
        {
            get
            {
                return _remainingTimeOfPhase;
            }
            set
            {
                _remainingTimeOfPhase = value;
                NotifyOfPropertyChange(() => RemainingTime);
                NotifyOfPropertyChange(() => Time);
            }
        }

        #endregion

        #region Methods
        public void Start()
        {
            if (_timerAction == null) {
                _timerAction = new BackgroundAction(RoundTimerTick);
                _timerAction.Start();
                RemainingTime = TimeSpan.FromSeconds(_roundDefinition.GetRoundTimeInSeconds());
                CurrentRound = 1;
            }
        }

        public void Stop()
        {
            if (_timerAction != null)
            {
                _timerAction.Terminate();
                _timerAction = null;
            }
        }


        private void RoundTimerTick(IBackgroundAction backgroundAction)
        {
            while (IsRunning)
            {
                Thread.Sleep(TIMER_INTERVAL);

                RemainingTime = RemainingTime.Subtract(TimeSpan.FromMilliseconds(TIMER_INTERVAL));

                if (RemainingTime < TimeSpan.Zero)
                {
                    ChangePhase();
                }

                NotifyOfPropertyChange(() => Time);
            }
        }

        private void ChangePhase()
        {
            switch (_phase)
            {
                case ManagerPhase.Round:
                    CountFinishedRounds++;
                    _phase = ManagerPhase.Pause;
                    RemainingTime = TimeSpan.FromSeconds(RoundDefinition.GetPauseTimeInSeconds());
                    break;
                case ManagerPhase.Pause:
                    CurrentRound++;
                    _phase = ManagerPhase.Round;
                    RemainingTime = TimeSpan.FromSeconds(RoundDefinition.GetRoundTimeInSeconds());
                    break;
                default:
                    throw new NotSupportedException("Unknown Phase not supported.");
            }
        }
        #endregion

        public void SetDefinition(IRoundDefinition definition)
        {
            Stop();
            _roundDefinition = definition;
        }
    }
}
