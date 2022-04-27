using UnityEngine;

namespace PixelDust.Audiophile
{
    [System.Serializable]
    public class AmbientTrackChangeEvent
    {
        [SerializeField]
        private AudioClip ambientClip;
        public AudioClip AmbientClip => ambientClip;

        [SerializeField]
        private float volume = 1;
        public float Volume => volume;

        [SerializeField]
        private float pitch = 1;
        public float Pitch => pitch;

        [SerializeField]
        private string mixerGroup;
        public string MixerGroup => mixerGroup;
    }
}