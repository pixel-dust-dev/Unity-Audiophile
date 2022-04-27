using UnityEngine;

namespace PixelDust.Audiophile
{
    [CreateAssetMenu(menuName = "Audiophile/Sound Event Collection Preset")]
    public class SoundEventCollectionPreset : ScriptableObject
    {
        [SerializeField]
        SoundEventCollectionData data;
        public SoundEventCollectionData Data => data;
    }
}