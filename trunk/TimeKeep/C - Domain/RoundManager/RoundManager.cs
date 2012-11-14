using System;

namespace TimeKeep.Domain.RoundManager
{
    using System.Media;
    using System.Threading;

    using Caliburn.Micro;

    using TimeKeep.Domain.SoundController;
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

        private readonly SoundManager _soundManager;

        #endregion

        #region Constructor

        public delegate void PhaseChangedHandler(object sender, ManagerPhase newPhase);
        public event PhaseChangedHandler OnPhaseChanged;
        
        public RoundManager(IRoundDefinition roundDef)
        {
            SetDefinition(roundDef);
            _soundManager = new SoundManager();
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

        /// <summary>
        /// Time since current phase start as string (00:00:00)
        /// </summary>
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

        
        /// <summary>
        /// Gets or sets the remaining time of the current phase as timespan
        /// </summary>
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

        /// <summary>
        /// Gets the current phase of the round manager
        /// </summary>
        public ManagerPhase CurrentPhase
        {
            get
            {
                return _phase;
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

        public void Reset() {
            _phase = ManagerPhase.Round;
            if (OnPhaseChanged != null)
            {
                OnPhaseChanged(this, _phase);
            }
            CountFinishedRounds = 0;
            CurrentRound = 0;
            RemainingTime = TimeSpan.Zero;
        }

        private void RoundTimerTick(IBackgroundAction backgroundAction)
        {
            while (IsRunning)
            {
                if (!backgroundAction.IsTerminated)
                {
                    Thread.Sleep(TIMER_INTERVAL);

                    RemainingTime = RemainingTime.Subtract(TimeSpan.FromMilliseconds(TIMER_INTERVAL));
                    ProcessSounds(RemainingTime);
                    if (RemainingTime <= TimeSpan.Zero) {
                        ChangePhase();
                    }

                    NotifyOfPropertyChange(() => Time);
                }
            }
        }

        private void ProcessSounds(TimeSpan time)
        { 
            _soundManager.ProcessSounds(time, _phase);
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

            if (OnPhaseChanged != null)
            {
                OnPhaseChanged(this, _phase);
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
