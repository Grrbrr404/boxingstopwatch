namespace TimeKeep.Application.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Media;
    using System.Net.Sockets;
    using System.Speech.Synthesis;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    using Caliburn.Micro;

    using TimeKeep.C.Domain;
    using TimeKeep.C.Domain.Extensions;
    using TimeKeep.Domain;
    using TimeKeep.Domain.RoundManager;
    using TimeKeep.Domain.SoundController;
    using TimeKeep.Properties;

    [Export]
    public sealed class ConfigViewModel : Screen
    {
        #region Constants and Fields
        /// <summary>
        ///     Defines the amount of templates that should be saved inside the template history
        /// </summary>
        private const int MAX_TEMPLATES_IN_HISTORY = 5;

        /// <summary>
        ///     Current maximum rounds
        /// </summary>
        private int _maxRounds;

        /// <summary>
        ///     Current pause minutes
        /// </summary>
        private int _pauseMinute;

        /// <summary>
        ///     Current pause seconds
        /// </summary>
        private int _pauseSecond;

        /// <summary>
        ///     Result that is returned of user accept changes by clicking OK button
        /// </summary>
        private IRoundDefinition _result;

        /// <summary>
        ///     Current round minutes
        /// </summary>
        private int _roundMinute;

        /// <summary>
        ///     Current round seconds
        /// </summary>
        private int _roundSecond;

        /// <summary>
        ///     Template collection, used for template history listview
        /// </summary>
        private ObservableCollection<RoundTemplate> _roundTemplateCollection;

        /// <summary>
        ///     Current use max rounds yes / no
        /// </summary>
        private bool _useMaxRounds;
        #endregion

        #region Constructors and Destructors
        [ImportingConstructor]
        public ConfigViewModel(IRoundDefinition currentlyUsedDefinition)
        {
            DisplayName = "Konfiguration";
            LoadFromDefinition(currentlyUsedDefinition);
            if (Settings.Default.RoundTemplates == null)
            {
                Settings.Default.RoundTemplates = new StringCollection();
            }

            RoundTemplateCollection = Settings.Default.RoundTemplates.ToRoundTemplateCollection();
        }
        #endregion

        #region Public Properties
        public IRoundDefinition DialogResult
        {
            get
            {
                return _result;
            }
        }

        /// <summary>
        /// Gets or sets the display background color for round phase.
        /// Value is stored in settings file as hex value
        /// </summary>
        public Color DisplayBackgroundColor
        {
            get
            {
                string color = Settings.Default.DisplayBackgroundColor;
                if (string.IsNullOrEmpty(color))
                {
                    color = "#FFFFFF";
                }
                return (Color)ColorConverter.ConvertFromString(color);
            }

            set
            {
                Settings.Default.DisplayBackgroundColor = value.ToString();
                Settings.Default.Save();
                NotifyOfPropertyChange(() => DisplayBackgroundColor);
            }
        }

        /// <summary>
        /// Gets or sets the display background color for pause phase.
        /// Value is stored in settings file as hex value
        /// </summary>
        public Color DisplayBackgroundColorPause
        {
            get
            {
                string color = Settings.Default.DisplayBackgroundColorPause;
                if (string.IsNullOrEmpty(color))
                {
                    color = "#FFFFFF";
                }
                return (Color)ColorConverter.ConvertFromString(color);
            }

            set
            {
                Settings.Default.DisplayBackgroundColorPause = value.ToString();
                Settings.Default.Save();
                NotifyOfPropertyChange(() => DisplayBackgroundColorPause);
            }
        }

        /// <summary>
        /// Gets or sets the display foreground color for round phase.
        /// Value is stored in settings file as hex value
        /// </summary>
        public Color FontColor
        {
            get
            {
                string color = Settings.Default.FontColor;
                if (string.IsNullOrEmpty(color))
                {
                    color = "#000000";
                }
                return (Color)ColorConverter.ConvertFromString(color);
            }

            set
            {
                Settings.Default.FontColor = value.ToString();
                Settings.Default.Save();
                NotifyOfPropertyChange(() => FontColor);
            }
        }

        /// <summary>
        /// Gets or sets the display foreground color for pause phase.
        /// Value is stored in settings file as hex value
        /// </summary>
        public Color FontColorPause
        {
            get
            {
                string color = Settings.Default.FontColorPause;
                if (string.IsNullOrEmpty(color))
                {
                    color = "#000000";
                }
                return (Color)ColorConverter.ConvertFromString(color);
            }

            set
            {
                Settings.Default.FontColorPause = value.ToString();
                Settings.Default.Save();
                NotifyOfPropertyChange(() => FontColorPause);
            }
        }

        /// <summary>
        /// Gets or sets maximum rounds
        /// </summary>
        public int MaxRounds
        {
            get
            {
                return _maxRounds;
            }
            set
            {
                _maxRounds = value;
                UseMaxRounds = _maxRounds > 0;
                NotifyOfPropertyChange(() => MaxRounds);
            }
        }

        /// <summary>
        /// Gets or sets pause minutes
        /// </summary>
        public int PauseMinute
        {
            get
            {
                return _pauseMinute;
            }
            set
            {
                _pauseMinute = value;
                NotifyOfPropertyChange(() => PauseMinute);
            }
        }

        /// <summary>
        /// Gets or sets pause seconds
        /// </summary>
        public int PauseSecond
        {
            get
            {
                return _pauseSecond;
            }
            set
            {
                _pauseSecond = value;
                NotifyOfPropertyChange(() => PauseSecond);
            }
        }

        /// <summary>
        /// Gets or sets round minutes
        /// </summary>
        public int RoundMinute
        {
            get
            {
                return _roundMinute;
            }

            set
            {
                _roundMinute = value;
                NotifyOfPropertyChange(() => RoundMinute);
            }
        }

        /// <summary>
        /// Gets or sets round seconds
        /// </summary>
        public int RoundSecond
        {
            get
            {
                return _roundSecond;
            }
            set
            {
                _roundSecond = value;
                NotifyOfPropertyChange(() => RoundSecond);
            }
        }

        /// <summary>
        /// Gets or private sets Template history
        /// </summary>
        public ObservableCollection<RoundTemplate> RoundTemplateCollection
        {
            get
            {
                return _roundTemplateCollection;
            }
            private set
            {
                _roundTemplateCollection = value;
                NotifyOfPropertyChange(() => RoundTemplateCollection);
            }
        }

        /// <summary>
        /// Gets or sets use maximum rounds
        /// </summary>
        public bool UseMaxRounds
        {
            get
            {
                return _useMaxRounds;
            }
            set
            {
                _useMaxRounds = value;
                NotifyOfPropertyChange(() => UseMaxRounds);

                if (!_useMaxRounds)
                {
                    _maxRounds = 0;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Executed if user clicks OK in Configuration Dialog
        /// </summary>
        public void AcceptChanges()
        {
            _result = new StaticRoundDefinition
            {
                MaxRounds = this.MaxRounds,
                RoundInSeconds = this.RoundMinute * 60 + this.RoundSecond,
                PauseInSeconds = this.PauseMinute * 60 + this.PauseSecond
            };

            AddDefinitionToHistory(_result);

            Settings.Default.Save();
            TryClose(true);
        }

        /// <summary>
        ///     Sets Properties provided by a definition
        /// </summary>
        /// <param name="definition"></param>
        public void LoadFromDefinition(IRoundDefinition definition)
        {
            var timeRounds = TimeSpan.FromSeconds(definition.GetRoundTimeInSeconds());
            var timePause = TimeSpan.FromSeconds(definition.GetPauseTimeInSeconds());

            RoundMinute = timeRounds.Minutes;
            RoundSecond = timeRounds.Seconds;
            PauseMinute = timePause.Minutes;
            PauseSecond = timePause.Seconds;
            MaxRounds = definition.GetMaxRounds();
        }

        /// <summary>
        ///     Event to load a round template from listview (ListView SelectionChanged event)
        /// </summary>
        /// <param name="source">The raiser of event</param>
        /// <param name="args">Args</param>
        public void LoadFromTemplate(object source, SelectionChangedEventArgs args)
        {
            if (args.AddedItems[0] is RoundTemplate)
            {
                var template = (RoundTemplate)args.AddedItems[0];
                LoadFromDefinition(template.Definition);
            }
        }

        /// <summary>
        ///     Event to load a template and close configuration dialog automatically (PreviewMouseDown Event)
        /// </summary>
        /// <param name="source">The raiser of event</param>
        /// <param name="args">Args</param>
        public void LoadFromTemplateAndCloseDialog(object source, MouseButtonEventArgs args)
        {
            if (args.ClickCount == 2)
            {
                var listv = source as ListView;
                if (listv != null)
                {
                    if (listv.SelectedItem != null)
                    {
                        var template = (RoundTemplate)listv.SelectedItem;
                        LoadFromDefinition(template.Definition);
                        AcceptChanges();
                    }
                }
            }
        }

        /// <summary>
        ///     Used on textboxes to prevent alpha input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PreviewTextInput(TextBox sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsNumber);
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Adds a round definition to the history
        /// </summary>
        /// <param name="definition">The definition</param>
        private void AddDefinitionToHistory(IRoundDefinition definition)
        {
            int count =
                RoundTemplateCollection.Count(
                    item =>
                    item.Definition.GetMaxRounds() == definition.GetMaxRounds()
                    && item.Definition.GetPauseTimeInSeconds() == definition.GetPauseTimeInSeconds()
                    && item.Definition.GetRoundTimeInSeconds() == definition.GetRoundTimeInSeconds());

            if (count == 0)
            {
                // Item config does not exist in template collection
                var itemToAdd = new RoundTemplate(definition);
                RoundTemplateCollection.Insert(0, itemToAdd);

                if (RoundTemplateCollection.Count > MAX_TEMPLATES_IN_HISTORY)
                {
                    // History is full, remove oldest item
                    RoundTemplateCollection.RemoveAt(RoundTemplateCollection.Count - 1);
                }

                Settings.Default.RoundTemplates = RoundTemplateCollection.ToStringCollection();
            }
        }
        #endregion
    }
}