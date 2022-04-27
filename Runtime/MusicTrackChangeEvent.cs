using UnityEngine;

namespace PixelDust.Audiophile
{
    [System.Serializable]
    public class MusicTrackChangeEvent
    {
        public AudioClip musicClip;
        public float volume = 1;
        public float pitch = 1;
    }
}