namespace TimeKeep.Domain.SoundController
{
    using System;
    using System.IO;
    using TimeKeep.Domain.RoundManager;


    /// <summary>
    ///     Class to create a sound definition for project resource roundend boxing bell
    /// </summary>
    public class RoundEndBoxingBellSoundDefinition : SoundDefinition
    {
        #region Constructors and Destructors

        /// <summary>
        /// Use this constructor if you want to create default boxing bell sound definition
        /// </summary>
        public RoundEndBoxingBellSoundDefinition()
        {
            _name = "Runden Ende Glocke";
            _phaseToPlaySound = ManagerPhase.Round;
            _beginPlayback = TimeSpan.Zero;
            _soundLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds\\roundend-bell.wav");
            SetRepeatDuration(TimeSpan.Zero);
        }
        #endregion
    }
}