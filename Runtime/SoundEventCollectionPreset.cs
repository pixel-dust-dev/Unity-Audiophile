using UnityEngine;

namespace PixelDust.Audiophile
{
    [CreateAssetMenu(menuName = "Audiophile/Sound Event Collection Preset")]
    public class SoundEventCollectionPreset : ScriptableObject
    {
        [SerializeField]
        SoundEventCollectionData data;
        public SoundEventCollectionData Data => data;

        public void Stop()
        {
            Data.Stop();
        }

        public void Play()
        {
            Data.Play();
        }

        public void PlayDelayed(float delay)
        {
            Data.Play(delay);
        }
    }
}