﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeKeep.Domain.SoundController
{
    using System.Diagnostics;
    using System.Media;
    using System.Threading;

    using TimeKeep.Domain.RoundManager;
    using TimeKeep.Foundation.Threading;
    using TimeKeep.Foundation.Threading.Interfaces;

    public class SoundManager
    {

        public struct SoundKey
        {
            public TimeSpan Time { get; set; }
            public ManagerPhase Phase { get; set; }
        }

        private readonly Dictionary<SoundKey, ISoundDefinition> _sounds;

        public SoundManager()
        {
            _sounds = new Dictionary<SoundKey, ISoundDefinition>();
            InitSounds();
        }

        private void InitSounds()
        {
            //AddSound(new RoundEndBoxingBellSoundDefinition(ManagerPhase.Round, TimeSpan.Zero));
            AddSound(new RoundEndBoxingBellSoundDefinition(ManagerPhase.Round, TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(10)));
        }

        private void AddSound(ISoundDefinition def)
        {
            var key = new SoundKey { Phase = def.PhaseToPlaySound, Time = def.BeginPlayback };
            if (!Sounds.ContainsKey(key))
            {
                Sounds.Add(key, def);
            }
        }

        public void ProcessSounds(TimeSpan origin, ManagerPhase phase)
        {
            var key = new SoundKey { Phase = phase, Time = origin };
            if (Sounds.ContainsKey(key))
            {
                var def = Sounds[key];
                if (def.IsRepeatPlaybackEnabled)
                {
                    var thread = new Thread(PlayRepeatSound);
                    thread.Start(def);
                }
                else
                {
                    PlaySound(def.SoundSource);
                }

            }
        }

        private void PlayRepeatSound(object param)
        {
            var def = param as ISoundDefinition;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var sp = new SoundPlayer(def.SoundSource))
            {
                while (stopwatch.Elapsed < def.RepeatDuration) {
                    sp.PlaySync();    
                }
            }

            stopwatch.Stop();
        }

        private void PlaySound(string soundLocation)
        {
            using (var sp = new SoundPlayer(soundLocation))
            {
                sp.Play();
            }
        }


        private void PlaySoundInBackgroundThread(IBackgroundAction action)
        {

        }

        public Dictionary<SoundKey, ISoundDefinition> Sounds
        {
            get
            {
                return _sounds;
            }
        }
    }
}