using UnityEngine;

namespace PixelDust.Audiophile
{
    [CreateAssetMenu(menuName = "Audiophile/Sound Event Preset")]
    public class SoundEventPreset : ScriptableObject
    {
        [SerializeField]
        SoundEventData data;
        public SoundEventData Data => data;
    }
}