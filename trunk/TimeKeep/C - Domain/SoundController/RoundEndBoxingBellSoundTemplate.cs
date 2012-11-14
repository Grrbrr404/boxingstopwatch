namespace TimeKeep.Domain.SoundController
{
    using System;
    using System.IO;

    using TimeKeep.Domain.RoundManager;

    /// <summary>
    ///     Class to create a sound definition for project resource roundend boxing bell
    /// </summary>
    public class RoundEndBoxingBellSoundDefinition : BaseSoundDefinition
    {
        #region Constructors and Destructors

        /// <summary>
        /// Use this constructor if you want to repeat the sound multiple times
        /// </summary>
        /// <param name="phaseToPlaySound">The phase where the should should be played</param>
        /// <param name="beginPlayback">The second to start playing the sound</param>
        /// <param name="repeatDuration">The sound will be repeated until this parameter is reached</param>
        public RoundEndBoxingBellSoundDefinition(
            ManagerPhase phaseToPlaySound, TimeSpan beginPlayback, TimeSpan repeatDuration)
        {
            _name = "Runden Ende Glocke";
            _phaseToPlaySound = phaseToPlaySound;
            _playSoundAtSecond = beginPlayback;
            SoundSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds\\roundend-bell.wav");
            SetRepeatDuration(repeatDuration);
        }

        /// <summary>
        /// Use this concrutor if you dont want to repeat the sound
        /// </summary>
        /// <param name="phaseToPlaySound"></param>
        /// <param name="beginPlayback"></param>
        public RoundEndBoxingBellSoundDefinition(ManagerPhase phaseToPlaySound, TimeSpan beginPlayback)
            : this(phaseToPlaySound, beginPlayback, TimeSpan.Zero) {}
        #endregion
    }
}