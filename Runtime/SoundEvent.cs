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
        private SoundEventData data = new SoundEventData();
        public SoundEventData Data => preset != null ? preset.Data : data;

        public void Stop()
        {
            Data.Stop();
        }

        public AudiophilePlayResult PlayAt(Vector3 position, float delay = 0, string overrideId = null, Transform followTransform = null)
        {
            return Data.PlayAt(position, delay, overrideId, followTransform);
        }

        public AudiophilePlayResult PlayAt(Transform transform, float delay = 0, string overrideId = null, bool follow = false)
        {
            Vector3 position = transform != null ? transform.position : Vector3.zero;
            return Data.PlayAt(position, delay, overrideId, follow ? transform : null);
        }

        public AudiophilePlayResult Play(float delay = 0, string overrideId = null)
        {
            return Data.PlayAt(Vector3.zero, delay, overrideId);
        }
    }
}