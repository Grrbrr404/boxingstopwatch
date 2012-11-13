
namespace TimeKeep.Application.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel.Composition;

    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using Caliburn.Micro;

    using TimeKeep.C.Domain;
    using TimeKeep.C.Domain.Extensions;
    using TimeKeep.Domain;
    using TimeKeep.Domain.Interfaces;
    using TimeKeep.Properties;

    [Export]
    public class ConfigViewModel : Screen
    {
        private const int MAX_TEMPLATE = 5;

        #region Fields
        private int _roundMinute;

        private int _roundSecond;

        private int _pauseMinute;

        private int _pauseSecond;

        private ObservableCollection<RoundTemplate> _roundTemplateCollection;

        private bool _useMaxRounds;

        private int _maxRounds;

        private IRoundDefinition _result;

        private Color _displayBackgroundColor;

        #endregion

        #region Properties Template Tab
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



        #endregion

        #region Properties Sound & Color Tab

        public Color DisplayBackgroundColor
        {
            get
            {
                var color = Settings.Default.DisplayBackgroundColor;
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

        public Color FontColor
        {
            get
            {
                var color = Settings.Default.FontColor;
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

        #endregion

        #region Properties Common
        public IRoundDefinition DialogResult
        {
            get
            {
                return _result;
            }
        }

        #endregion

        #region Constructor
        [ImportingConstructor]
        public ConfigViewModel(IRoundDefinition currentlyUsedDefinition)
        {
            LoadFromDefinition(currentlyUsedDefinition);
            if (Properties.Settings.Default.RoundTemplates == null)
            {
                Properties.Settings.Default.RoundTemplates = new StringCollection();
            }

            RoundTemplateCollection = Properties.Settings.Default.RoundTemplates.ToRoundTemplateCollection();
        }

        #endregion

        public void PreviewTextInput(TextBox sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsNumber);
        }

        public void LoadFromTemplate(object source, SelectionChangedEventArgs args)
        {
            if (args.AddedItems[0] is RoundTemplate)
            {
                var template = (RoundTemplate)args.AddedItems[0];
                LoadFromDefinition(template.Definition);
            }
        }

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



        public void AcceptChanges()
        {
            _result = new StaticRoundDefinition
            {
                MaxRounds = MaxRounds,
                RoundInSeconds = RoundMinute * 60 + RoundSecond,
                PauseInSeconds = PauseMinute * 60 + PauseSecond
            };

            AddDefinitionToHistory(_result);

            Properties.Settings.Default.Save();
            TryClose(true);
        }

        private void AddDefinitionToHistory(IRoundDefinition definition)
        {
            var count = RoundTemplateCollection.Count(
                item =>
                item.Definition.GetMaxRounds() == definition.GetMaxRounds()
                && item.Definition.GetPauseTimeInSeconds() == definition.GetPauseTimeInSeconds()
                && item.Definition.GetRoundTimeInSeconds() == definition.GetRoundTimeInSeconds());

            if (count == 0)
            {
                // Item config does not exist in template collection
                var itemToAdd = new RoundTemplate(definition);
                RoundTemplateCollection.Insert(0, itemToAdd);

                if (RoundTemplateCollection.Count > MAX_TEMPLATE)
                {
                    // History is full, remove oldest item
                    RoundTemplateCollection.RemoveAt(RoundTemplateCollection.Count - 1);
                }

                Properties.Settings.Default.RoundTemplates = RoundTemplateCollection.ToStringCollection();
            }
        }
    }
}
