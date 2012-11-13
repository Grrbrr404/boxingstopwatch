namespace TimeKeep.Application.ViewModels
{
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Input;

    using Caliburn.Micro;

    using TimeKeep.Domain;
    using System.Threading;

    /// <summary>
    /// The shell view model.
    /// </summary>
    [Export(typeof(IShell))]
    public class ShellViewModel : PropertyChangedBase
    {

        #region Constants and Fields

        /// <summary>
        /// The _is fullscreen.
        /// </summary>
        private bool _isFullscreen;

        /// <summary>
        ///   Class that calculates remaining time of round and pause
        /// </summary>
        private RoundManager _roundManager = new RoundManager(new PropertyRoundDefinition());

        /// <summary>
        /// The _window state.
        /// </summary>
        private WindowState _windowState;

        /// <summary>
        /// The _window style.
        /// </summary>
        private WindowStyle _windowStyle;

        private readonly IWindowManager _windowManager;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public ShellViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            SetWindowMode();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets RM.
        /// </summary>
        public RoundManager RM
        {
            get
            {
                return _roundManager;
            }
        }

        /// <summary>
        /// Gets or sets WinState.
        /// </summary>
        public WindowState WinState
        {
            get
            {
                return _windowState;
            }

            set
            {
                _windowState = value;
                NotifyOfPropertyChange(() => WinState);
            }
        }

        /// <summary>
        /// Gets or sets WinStyle.
        /// </summary>
        public WindowStyle WinStyle
        {
            get
            {
                return _windowStyle;
            }

            set
            {
                _windowStyle = value;
                NotifyOfPropertyChange(() => WinStyle);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens Config Window
        /// </summary>
        public void ShowConfigWindow()
        {
            var configDialog = new ConfigViewModel(RM.RoundDefinition);
            var acceptChanges = _windowManager.ShowDialog(configDialog);
            if (acceptChanges.HasValue && acceptChanges.Value)
            {
                RM.SetDefinition(configDialog.DialogResult);
                RM.Stop();
                RM.Reset();
            }
        }

        /// <summary>
        /// Executed ShellView is closed
        /// </summary>
        public void OnClosing()
        {
            if (_roundManager.IsRunning)
            {
                _roundManager.Stop();
            }
        }

        /// <summary>
        /// The on key up.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.F11 || (e.Key == Key.Escape && _isFullscreen))
            {
                ToggleFullscreen();
            }
        }

        /// <summary>
        /// The start timer.
        /// </summary>
        public void StartTimer()
        {
            _roundManager.Start();
        }

        /// <summary>
        /// The stop timer.
        /// </summary>
        public void StopTimer()
        {
            _roundManager.Stop();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The set fullscreen mode.
        /// </summary>
        private void SetFullscreenMode()
        {
            WinState = WindowState.Maximized;
            WinStyle = WindowStyle.None;
            _isFullscreen = true;
        }

        /// <summary>
        /// The set window mode.
        /// </summary>
        private void SetWindowMode()
        {
            WinState = WindowState.Normal;
            WinStyle = WindowStyle.SingleBorderWindow;
            _isFullscreen = false;
        }

        /// <summary>
        /// The toggle fullscreen.
        /// </summary>
        private void ToggleFullscreen()
        {
            if (_isFullscreen)
            {
                SetWindowMode();
            }
            else
            {
                SetFullscreenMode();
            }
        }

        #endregion
    }
}