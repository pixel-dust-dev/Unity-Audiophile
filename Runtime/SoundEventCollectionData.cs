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

        public void PlayAt(Vector3 position, ulong delay = 0, string overrideId = null)
        {
            soundEventCollection.GetRandom()?.PlayAt(position, delay, overrideId != null ? overrideId : SoundId);
        }
    }
}