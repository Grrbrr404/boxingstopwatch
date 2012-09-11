namespace TimeKeep.Application.ViewModels {
  using System;
  using System.ComponentModel.Composition;
  using System.Threading;
  using System.Windows;
  using System.Windows.Input;

  using Caliburn.Micro;

  using TimeKeep.Domain;

  [Export(typeof(IShell))]
  public class ShellViewModel : PropertyChangedBase {


    /// <summary>
    /// Class that calculates remaining time of round and pause
    /// </summary>
    private RoundManager _roundManager = new RoundManager(new PropertyRoundDefinition());

    private WindowState _windowState;

    private WindowStyle _windowStyle;

    private bool _isFullscreen;

    public ShellViewModel() {
      SetWindowMode();
    }

    public void OnClosing() {
      if (_roundManager.IsRunning) {
        _roundManager.StopTimer();
      }
    }

    private void ToggleFullscreen() {
      if (_isFullscreen) {
        SetWindowMode();
      }
      else {
        SetFullscreenMode();
      }
    }

    private void SetWindowMode() {
      WinState = WindowState.Normal;
      WinStyle = WindowStyle.SingleBorderWindow;
      _isFullscreen = false;
    }

    private void SetFullscreenMode() {
      WinState = WindowState.Maximized;
      WinStyle = WindowStyle.None;
      _isFullscreen = true;
    }

    public void OnKeyUp(KeyEventArgs e) {
      if (e.Key == Key.F11) {
        ToggleFullscreen();
      }
    }

    public RoundManager RM {
      get {
        return _roundManager;
      }
    }

    public WindowStyle WinStyle {
      
      get {
        return _windowStyle;
      }
      
      set {
        _windowStyle = value;
        NotifyOfPropertyChange(() => WinStyle);
      }
    }

    public WindowState WinState {
      
      get {
        return _windowState;
      }

      set {
        _windowState = value;
        NotifyOfPropertyChange(() => WinState);
      }
    }

    public void StartTimer() {
     _roundManager.StartTimer();
    }

    public void StopTimer() {
      _roundManager.StopTimer();
    }
  }
}
