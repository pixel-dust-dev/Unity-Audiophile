using System;
using UnityEngine;
using WeightedObjects;

namespace PixelDust.Audiophile
{
    [Serializable]
    public class SoundEventCollectionData
    {
        [SerializeField]
        private WeightedObjectCollection<SoundEvent> soundEventCollection = new WeightedObjectCollection<SoundEvent>();
        public WeightedObjectCollection<SoundEvent> SoundEventCollection => soundEventCollection;

        [SerializeField]
        private string soundId;
        public string SoundId => soundId;

        public AudiophilePlayResult Play()
        {
            return PlayAt(Vector3.zero);
        }

        public AudiophilePlayResult Play(float delay)
        {
            return PlayAt(Vector3.zero, delay);
        }

        public AudiophilePlayResult PlayAt(Vector3 position, float delay = 0, string overrideId = null)
        {
            return soundEventCollection.GetRandom()?.PlayAt(position, delay, overrideId != null ? overrideId : SoundId);
        }

        public void Stop()
        {
            SoundManager.StopSound(this.SoundId);
        }
    }
}