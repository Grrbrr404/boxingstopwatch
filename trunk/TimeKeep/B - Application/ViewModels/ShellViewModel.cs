namespace TimeKeep.Application.ViewModels
{
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Input;

    using Caliburn.Micro;

    using TimeKeep.Domain.RoundManager;
    using TimeKeep.Properties;

    using System.Windows.Media;
    using TimeKeep.Domain.WebControlService;
    using System.Net;
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
        private readonly RoundManager _roundManager;

        /// <summary>
        /// The _window state.
        /// </summary>
        private System.Windows.WindowState _windowState;

        /// <summary>
        /// The _window style.
        /// </summary>
        private WindowStyle _windowStyle;


        private WebServer _webControlService;

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
            _roundManager = new RoundManager(new PropertyRoundDefinition());
            _roundManager.OnPhaseChanged += DoRoundManagerPhaseChanged;
            _webControlService = new WebServer(SendResponse, "http://*:8080/");
            _webControlService.Run();

            SetWindowMode();
        }

        private string SendResponse(HttpListenerRequest request)
        {
            var result = string.Format("<HTML><HEAD></HEAD><BODY><div style='padding:5px;'><a href='?action=start'/><input type='button' value='Start' style='width:100%;height:40%;'/></a><div style='height:15%'>&nbsp;</div><a href='?action=stop'/><input type='button' value='Stop' style='width:100%;height:40%;'/></a></div></HTML>");

            if (request.QueryString["action"] == "stop")
            {
                RM.Stop();
                Thread.Sleep(100);
                RM.Reset();
            }
            else if (request.QueryString["action"] == "start")
            {
                RM.Start();
            }
            return result;

        }

        private void DoRoundManagerPhaseChanged(object sender, ManagerPhase newPhase)
        {
            NotifyOfPropertyChange(() => WindowBackgroundColor);
            NotifyOfPropertyChange(() => FontColor);
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
        public System.Windows.WindowState WinState
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

        public Color WindowBackgroundColor
        {
            get
            {
                var color = RM.CurrentPhase == ManagerPhase.Round ? Settings.Default.DisplayBackgroundColor : Settings.Default.DisplayBackgroundColorPause;
                if (string.IsNullOrEmpty(color))
                {
                    color = "#FFFFFF";
                }
                return (Color)ColorConverter.ConvertFromString(color);
            }
        }

        public Color FontColor
        {
            get
            {
                var color = RM.CurrentPhase == ManagerPhase.Round ? Settings.Default.FontColor : Settings.Default.FontColorPause;
                if (string.IsNullOrEmpty(color))
                {
                    color = "#000000";
                }
                return (Color)ColorConverter.ConvertFromString(color);
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
                Thread.Sleep(100);
                RM.Reset();
                NotifyOfPropertyChange(() => WindowBackgroundColor);
                NotifyOfPropertyChange(() => FontColor);
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

            _webControlService.Stop();
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
            Thread.Sleep(100);
            _roundManager.Reset();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The set fullscreen mode.
        /// </summary>
        private void SetFullscreenMode()
        {
            WinState = System.Windows.WindowState.Maximized;
            WinStyle = WindowStyle.None;
            _isFullscreen = true;
        }

        /// <summary>
        /// The set window mode.
        /// </summary>
        private void SetWindowMode()
        {
            WinState = System.Windows.WindowState.Normal;
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