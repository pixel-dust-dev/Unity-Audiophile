using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelDust.Audiophile
{
    [ExecuteInEditMode]
    public class AudiophilePlayer : MonoBehaviour
    {
        public bool loop;
        public AudioSource audioSource;

        bool isPlaying = false;
        public bool IsPlaying => isPlaying;

        SoundEventData seData = null;

        public void Awake()
        {
            audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        public void Play(SoundEventData soundEventData)
        {
            seData = soundEventData;
            this.loop = soundEventData.Loop;
            audioSource.loop = false;

            audioSource.volume = UnityEngine.Random.Range(soundEventData.StandardSettings.MinVolume, soundEventData.StandardSettings.MaxVolume);
            audioSource.pitch = UnityEngine.Random.Range(soundEventData.StandardSettings.MinPitch, soundEventData.StandardSettings.MaxPitch);
            audioSource.outputAudioMixerGroup = SoundManager.GetMixerGroup(soundEventData.StandardSettings.Group);

            //Spatial Type
            audioSource.spatialBlend = soundEventData.SpatialSettings.Is3D ? 1 : 0;

            //3D
            audioSource.dopplerLevel = soundEventData.SpatialSettings.DopplerLevel;
            audioSource.spread = soundEventData.SpatialSettings.Spread;
            audioSource.rolloffMode = soundEventData.SpatialSettings.VolumeRolloff;
            audioSource.minDistance = soundEventData.SpatialSettings.MinDistance;
            audioSource.maxDistance = soundEventData.SpatialSettings.MaxDistance;

            //2D
            audioSource.panStereo = UnityEngine.Random.Range(soundEventData.SpatialSettings.StereoPanMin, soundEventData.SpatialSettings.StereoPanMax);

            //Advanced
            audioSource.bypassEffects = soundEventData.AdvancedSettings.BypassEffects;
            audioSource.bypassListenerEffects = soundEventData.AdvancedSettings.BypassListenerEffects;
            audioSource.bypassReverbZones = soundEventData.AdvancedSettings.BypassReverbZones;
            audioSource.reverbZoneMix = soundEventData.AdvancedSettings.ReverbZoneMix;

            AudioClip clip = soundEventData.AudioClips.GetRandom();

            audioSource.clip = clip;

            if (clip != null)
            {
                this.gameObject.name = clip.name + " - vol: " + audioSource.volume + " - pitch: " + audioSource.pitch;
                audioSource.Play();
                this.isPlaying = true;
            }
        }

        public void Update()
        {
            Debug.Log("Playing");
            if(isPlaying)
            {
                if(!this.audioSource.isPlaying)
                {
                    if (loop)
                    {
                        Play(seData);
                        return;
                    }
                    Stop();
                }
            }
        }

        public void Stop()
        {
            this.audioSource.Stop();
            this.isPlaying = false;
        }
    }
}
