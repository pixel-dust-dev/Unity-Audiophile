using UnityEngine;

namespace PixelDust.Audiophile
{
    [CreateAssetMenu(menuName = "Audiophile/Sound Event Preset")]
    public class SoundEventPreset : ScriptableObject
    {
        [SerializeField]
        SoundEventData data;
        public SoundEventData Data => data;

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

        public AudiophilePlayResult PlayAt(Vector3 position, float delay = 0, string overrideId = null)
        {
            return Data.PlayAt(position, delay, overrideId);
        }

        public AudiophilePlayResult PlayAt(Transform transform, float delay = 0, string overrideId = null)
        {
            Vector3 position = transform != null ? transform.position : Vector3.zero;
            return Data.PlayAt(position, delay, overrideId);
        }

        public AudiophilePlayResult Play(float delay = 0, string overrideId = null)
        {
            return Data.PlayAt(Vector3.zero, delay, overrideId);
        }
    }
}