using UnityEngine;
using UnityEngine.Serialization;

namespace PixelDust.Audiophile
{
    [System.Serializable]
    public class SoundEventCollection
    {
        [FormerlySerializedAs("soundEventCollectionPreset")]
        [SerializeField]
        private SoundEventCollectionPreset preset;

        //[SerializeReference] Was too much trouble
        [SerializeField]
        private SoundEventCollectionData data;
        public SoundEventCollectionData Data => preset != null ? preset.Data : data;

        public void Play(string overrideId = null, ulong delay = 0)
        {
            PlayAt(Vector3.zero, delay, overrideId);
        }

        public void PlayAt(Vector3 position, ulong delay = 0, string overrideId = null)
        {
            Data.PlayAt(position, delay, overrideId);
        }

        public void PlayAt(Transform transform, ulong delay = 0, string overrideId = null)
        {
            Vector3 position = transform != null ? transform.position : Vector3.zero;
            PlayAt(position, delay, overrideId);
        }
    }
}