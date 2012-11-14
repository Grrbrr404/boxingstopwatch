namespace TimeKeep.Domain.SoundController
{
    using System;

    using TimeKeep.Domain.RoundManager;

    public interface ISoundDefinition {

        string SoundLocation { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Defines the phase to play the sound in
        /// </summary>
        ManagerPhase PhaseToPlaySound { get; }

        /// <summary>
        /// Time to start playback
        /// </summary>
        TimeSpan BeginPlayback { get; }

        /// <summary>
        /// Defines the time for loop play
        /// </summary>
        TimeSpan RepeatDuration { get; }

        /// <summary>
        /// Gets or sets if repeat playbacks is enabled
        /// </summary>
        bool IsRepeatPlaybackEnabled { get; set; }

    }
}
