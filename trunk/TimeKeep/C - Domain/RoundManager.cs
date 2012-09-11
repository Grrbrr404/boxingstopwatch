using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeKeep.Domain {
  using System.ComponentModel;
  using System.Threading;

  using Caliburn.Micro;

  using TimeKeep.Properties;

  public class RoundManager : PropertyChangedBase {

    private const int TIMER_INTERVAL = 100;
    
    #region variables
    
    /// <summary>
    /// Round definition for this RoundManger instance.
    /// </summary>
    private IRoundDefinition _roundDefinition;
    
    /// <summary>
    /// Rounds finished
    /// </summary>
    private uint _countFinishedRounds = 0;

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
    public uint CountFinishedRounds {
      get {
        return _countFinishedRounds;
      }
      private set {
        _countFinishedRounds = value;
        NotifyOfPropertyChange(() => CountFinishedRounds);
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
          _phase = ManagerPhase.Pause;
          _remainingTimeOfPhase = TimeSpan.FromSeconds(_roundDefinition.GetPauseTimeInSeconds());
          break;
        case ManagerPhase.Pause:
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
