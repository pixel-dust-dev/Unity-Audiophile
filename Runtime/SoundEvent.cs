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

        public AudiophilePlayResult PlayAt(Vector3 position, float delay = 0, string overrideId = null)
        {
            return SoundManager.ProcessSound(this, position, delay, overrideId);
        }

        public AudiophilePlayResult PlayAt(Transform transform, float delay = 0, string overrideId = null)
        {
            Vector3 position = transform != null ? transform.position : Vector3.zero;
            return PlayAt(position, delay, overrideId);
        }

        public AudiophilePlayResult Play(float delay = 0, string overrideId = null)
        {
            return PlayAt(Vector3.zero, delay, overrideId);
        }
    }
}