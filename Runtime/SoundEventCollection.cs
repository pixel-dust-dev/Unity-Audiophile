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

        public void Play(string overrideId = null)
        {
            PlayAt(Vector3.zero, overrideId);
        }

        public void PlayAt(Vector3 position, string overrideId = null)
        {
            Data.PlayAt(position, overrideId);
        }

        public void PlayAt(Transform transform, string overrideId = null)
        {
            Vector3 position = transform != null ? transform.position : Vector3.zero;
            PlayAt(position, overrideId);
        }
    }
}