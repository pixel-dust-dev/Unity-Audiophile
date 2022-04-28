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

        public AudiophilePlayResult PlayAt(Vector3 position, string overrideId = null)
        {
            return SoundManager.ProcessSound(this, position, overrideId);
        }

        public AudiophilePlayResult PlayAt(Transform transform, string overrideId = null)
        {
            Vector3 position = transform != null ? transform.position : Vector3.zero;
            return PlayAt(position, overrideId);
        }

        public AudiophilePlayResult Play(string overrideId = null)
        {
            return PlayAt(Vector3.zero, overrideId);
        }
    }
}