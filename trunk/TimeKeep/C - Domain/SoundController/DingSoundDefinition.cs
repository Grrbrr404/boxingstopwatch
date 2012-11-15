namespace TimeKeep.Domain.SoundController
{
    using System;
    using System.IO;
    using TimeKeep.Domain.RoundManager;


    /// <summary>
    ///     Class to create a sound definition for project resource roundend boxing bell
    /// </summary>
    public class DingSoundDefinition : SoundDefinition
    {
        #region Constructors and Destructors

        /// <summary>
        /// Use this constructor if you want to create default boxing bell sound definition
        /// </summary>
        public DingSoundDefinition(ManagerPhase phase = ManagerPhase.Round)
        {
            _name = "Cowbell Countdown";
            _phaseToPlaySound = phase;
            _beginPlayback = TimeSpan.FromSeconds(3);
            _soundLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds\\ding.wav");
            SetRepeatDuration(TimeSpan.FromSeconds(3));
        }
        #endregion
    }
}