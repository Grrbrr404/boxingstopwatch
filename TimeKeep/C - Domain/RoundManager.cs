using System;

namespace TimeKeep.Domain {
  using System.Threading;

  using Caliburn.Micro;

  /// <summary>
  /// Timer class with start / stop functionality.
  /// </summary>
  public class RoundManager : PropertyChangedBase {

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
    /// Indicates if the timer thread is currently running or not
    /// </summary>
    private volatile bool _timerStopRequested = false;

    private volatile bool _timerIsRunning = false;

    /// <summary>
    /// Timer thread. Used to count down rounds / pause
    /// </summary>
    private Thread _timerThread;

    private ManagerPhase _phase = ManagerPhase.Round;

    private TimeSpan _remainingTimeOfPhase = TimeSpan.Zero;

    #endregion

    #region Constructor
    public RoundManager(IRoundDefinition roundDef) {
      _roundDefinition = roundDef;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Rounds finished
    /// </summary>
    public int CountFinishedRounds {
      get {
        return _countFinishedRounds;
      }
      private set {
        _countFinishedRounds = value;
        NotifyOfPropertyChange(() => CountFinishedRounds);
        NotifyOfPropertyChange(() => RoundText);
      }
    }

    public string RoundText {
      get {
        var roundOffset = HasMaxRounds ? _roundDefinition.GetMaxRounds().ToString() : "\u221E";
        return string.Format("{0} / {1}", CountFinishedRounds, roundOffset);
      }
    }

    public int CurrentRound {
      get {
        return _currentRound;
      }
      private set {
        _currentRound = value;
        NotifyOfPropertyChange(() => CurrentRound);
      }
    }

    /// <summary>
    /// Indicates if this RoundManager does use max rounds or not
    /// </summary>
    public bool HasMaxRounds {
      get {
        return _roundDefinition.GetMaxRounds() > 0;
      }
    }

    /// <summary>
    /// Indicates if the internal timer thread is currently running or not
    /// </summary>
    public bool IsRunning {
      
      get {
        return _timerIsRunning;
      }

      private set {
        _timerIsRunning = value;
        NotifyOfPropertyChange(() => IsRunning);
      }
    }

    public string Time {
      get {
        
        return string.Format(
          "{0:00}:{1:00}:{2:00}",
          _remainingTimeOfPhase.Minutes,
          _remainingTimeOfPhase.Seconds,
          _remainingTimeOfPhase.Milliseconds / 10.0);
      }
    }

    #endregion

    #region Methods
    private void RequestTimerStop() {
      _timerStopRequested = true;
    }

    public void StartTimer() {

      if (!IsRunning) {
        if (_timerThread == null) {
          _timerStopRequested = false;
          _timerThread = new Thread(new ThreadStart(RoundTimerTick));
        }
      
        if (_remainingTimeOfPhase == TimeSpan.Zero) {
          _remainingTimeOfPhase = TimeSpan.FromSeconds(_roundDefinition.GetRoundTimeInSeconds());
          CurrentRound = 1;
        }
      
        _timerThread.Start();
      }
    }

    public void StopTimer() {
      RequestTimerStop();
      _timerThread = null;
    }


    private void RoundTimerTick() {
      
      IsRunning = true;
      
      while (!_timerStopRequested) {
        Thread.Sleep(TIMER_INTERVAL);
        
        _remainingTimeOfPhase = _remainingTimeOfPhase.Subtract(TimeSpan.FromMilliseconds(TIMER_INTERVAL));
        
        if (_remainingTimeOfPhase < TimeSpan.Zero) {
          ChangePhase();
        }

        NotifyOfPropertyChange(() => Time);
      }

      IsRunning = false;
    }

    private void ChangePhase() {
      switch (_phase) {
        case ManagerPhase.Round:
          CountFinishedRounds++;
          _phase = ManagerPhase.Pause;
          _remainingTimeOfPhase = TimeSpan.FromSeconds(_roundDefinition.GetPauseTimeInSeconds());
          break;
        case ManagerPhase.Pause:
          CurrentRound++;
          _phase = ManagerPhase.Round;
          _remainingTimeOfPhase = TimeSpan.FromSeconds(_roundDefinition.GetRoundTimeInSeconds());
          break;
        default:
          throw new NotSupportedException("Unknown Phase not supported.");
      }
    }
    #endregion
  }
}
