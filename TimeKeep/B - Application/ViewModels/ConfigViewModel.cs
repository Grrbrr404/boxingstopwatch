using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeKeep.Application.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Caliburn.Micro;

    using TimeKeep.C.Domain;
    using TimeKeep.Domain;

    [Export]
    public class ConfigViewModel : PropertyChangedBase
    {
        private int _roundMinute;

        private readonly ObservableCollection<RoundTemplate> _roundTemplateCollection;

        private bool _useMaxRounds;

        private int _maxRounds;

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
                    MaxRounds = 0;
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
                NotifyOfPropertyChange(() => MaxRounds);
            }
        }

        public ConfigViewModel()
        {
            RoundMinute = 7;
            _roundTemplateCollection = new ObservableCollection<RoundTemplate>
            {
                new RoundTemplate(new StaticRoundDefinition {PauseInSeconds = 50, RoundInSeconds = 10}),
                new RoundTemplate(new StaticRoundDefinition {PauseInSeconds = 20, RoundInSeconds = 5}),
                new RoundTemplate(new StaticRoundDefinition {PauseInSeconds = 30, RoundInSeconds = 80}),
                new RoundTemplate(new StaticRoundDefinition {PauseInSeconds = 40, RoundInSeconds = 100})
            };
        }

        public void PreviewTextInput(TextBox sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsNumber);
        }
    }
}
