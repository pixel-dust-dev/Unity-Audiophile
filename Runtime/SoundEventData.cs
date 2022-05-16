using UnityEngine;
using WeightedObjects;

namespace PixelDust.Audiophile
{
    [System.Serializable]
    public class SoundEventData
    {
        [SerializeField]
        private WeightedObjectCollection<AudioClip> audioClips = new WeightedObjectCollection<AudioClip>();
        public WeightedObjectCollection<AudioClip> AudioClips => audioClips;

        [SerializeField]
        private bool loop = false;
        public bool Loop => loop;

        [SerializeField]
        private string soundId;
        public string SoundId => soundId;

        [SerializeField]
        private StandardSettings standardSettings;
        public StandardSettings StandardSettings => standardSettings;

        [SerializeField]
        private AdvancedSettings advancedSettings;
        public AdvancedSettings AdvancedSettings => advancedSettings;

        [SerializeField]
        private SpatialSettings spatialSettings;
        public SpatialSettings SpatialSettings => spatialSettings;

        public void Stop()
        {
            SoundManager.StopSound(this.SoundId);
        }

        public AudiophilePlayResult PlayAt(Vector3 position, float delay = 0, string overrideId = null)
        {
            return SoundManager.ProcessSound(this, position, delay, overrideId);
        }

        public AudiophilePlayResult PlayAt(Transform transform, float delay = 0, string overrideId = null)
        {
            Vector3 position = transform != null ? transform.position : Vector3.zero;
            return PlayAt(position, delay, overrideId);
        }

        public AudiophilePlayResult Play(float delay = 0, string overrideId = null)
        {
            return PlayAt(Vector3.zero, delay, overrideId);
        }
    }

    [System.Serializable]
    public class StandardSettings
    {
        [Range(0, 2)]
        [SerializeField]
        private float minPitch = 1;
        public float MinPitch => minPitch;

        [Range(0, 2)]
        [SerializeField]
        private float maxPitch = 1;
        public float MaxPitch => maxPitch;

        [Range(0, 1)]
        [SerializeField]
        private float minVolume = 1;
        public float MinVolume => minVolume;

        [Range(0, 1)]
        [SerializeField]
        private float maxVolume = 1;
        public float MaxVolume => maxVolume;

        [SerializeField]
        private UnityEngine.Audio.AudioMixerGroup group;
        public UnityEngine.Audio.AudioMixerGroup Group => group;
    }

    [System.Serializable]
    public class SpatialSettings
    {
        #region 2DSettings
        [SerializeField]
        [Range(-1, 1)]
        private float stereoPanMin = 0;
        public float StereoPanMin => stereoPanMin;

        [SerializeField]
        [Range(-1, 1)]
        private float stereoPanMax = 0;
        public float StereoPanMax => stereoPanMax;
        #endregion

        #region 3DSettings
        [SerializeField]
        private bool is3D = false;
        public bool Is3D => is3D;

        [SerializeField]
        [Range(0, 5)]
        private float dopplerLevel = 1;
        public float DopplerLevel => dopplerLevel;

        [SerializeField]
        [Range(0, 360)]
        private float spread = 0;
        public float Spread => spread;

        [SerializeField]
        private AudioRolloffMode volumeRolloff;
        public AudioRolloffMode VolumeRolloff => volumeRolloff;

        [SerializeField]
        private float minDistance = 1;
        public float MinDistance => minDistance;

        [SerializeField]
        private float maxDistance = 500;
        public float MaxDistance => maxDistance;
        #endregion
    }

    [System.Serializable]
    public class AdvancedSettings
    {
        [SerializeField]
        private bool bypassEffects = false;
        public bool BypassEffects => bypassEffects;

        [SerializeField]
        private bool bypassListenerEffects = false;
        public bool BypassListenerEffects => bypassListenerEffects;

        [SerializeField]
        private bool bypassReverbZones = false;
        public bool BypassReverbZones => bypassReverbZones;

        [SerializeField]
        [Range(0, 1.1f)]
        private float reverbZoneMix = 1;
        public float ReverbZoneMix => reverbZoneMix;
    }
}