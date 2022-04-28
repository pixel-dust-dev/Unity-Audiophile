using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PixelDust.Audiophile
{
    [System.Serializable]
    public class SoundEvent
    {
        [FormerlySerializedAs("soundEventPreset")]
        [SerializeField]
        private SoundEventPreset preset;

        //[SerializeReference] Was too much trouble
        [SerializeField]
        private SoundEventData data;
        public SoundEventData Data => preset != null ? preset.Data : data;

        public void Stop()
        {
            SoundManager.StopSound(Data.SoundId);
        }

        public void PlayAt(Vector3 position, string overrideId = null)
        {
            SoundManager.ProcessSound(this, position, overrideId);
        }

        public void PlayAt(Transform transform, string overrideId = null)
        {
            Vector3 position = transform != null ? transform.position : Vector3.zero;
            PlayAt(position, overrideId);
        }

        public void Play(string overrideId = null)
        {
            PlayAt(Vector3.zero, overrideId);
        }
    }
}