namespace TimeKeep.Domain.SoundController
{
    using System;
    using System.IO;
    using TimeKeep.Domain.RoundManager;


    /// <summary>
    ///     Class to create a sound definition for project resource roundend boxing bell
    /// </summary>
    public class TrippleWoodTockSoundDefinition : SoundDefinition
    {
        #region Constructors and Destructors

        /// <summary>
        /// Use this constructor if you want to create default boxing bell sound definition
        /// </summary>
        public TrippleWoodTockSoundDefinition()
        {
            _name = "Beep Countdown";
            _phaseToPlaySound = ManagerPhase.Round;
            _beginPlayback = TimeSpan.FromSeconds(10);
            _soundLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds\\wood-trippletock.wav");
            SetRepeatDuration(TimeSpan.Zero);
        }
        #endregion
    }
}