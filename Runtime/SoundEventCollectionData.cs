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

        [SerializeField]
        private string soundId;
        public string SoundId => soundId;

        public void PlayAt(Vector3 position, string overrideId = null)
        {
            soundEventCollection.GetRandom()?.PlayAt(position, overrideId != null ? overrideId : SoundId);
        }
    }
}