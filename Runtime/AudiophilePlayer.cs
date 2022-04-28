﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelDust.Audiophile
{
    public class AudiophilePlayResult
    {
        private AudiophilePlayer audiophilePlayer;
        private AudiophilePlayer AudiophilePlayer => audiophilePlayer;

        public float? Volume
        {
            get
            {
                return audiophilePlayer ? audiophilePlayer.audioSource.volume : 0;
            }
            set
            {
                this.audiophilePlayer.SetOverrideVolume(value);
            }
        }

        public float? Pitch
        {
            get
            {
                return audiophilePlayer ? audiophilePlayer.audioSource.pitch : 0;
            }
            set
            {
                this.audiophilePlayer.SetOverridePitch(value);
            }
        }

        public event Action onLoop;

        public AudiophilePlayResult(AudiophilePlayer audiophilePlayer)
        {
            this.audiophilePlayer = audiophilePlayer;
            this.audiophilePlayer.onStopped += OnStopped;
            this.audiophilePlayer.onLooped += () => onLoop?.Invoke();
        }

        private void OnStopped()
        {
            audiophilePlayer = null;
        }

        public void Stop()
        {
            if (this.audiophilePlayer)
            {
                this.audiophilePlayer.Stop();
            }
            else
            {
                Debug.Log("Audiophile - Audiophile player is null");
            }
        }
    }

    [ExecuteInEditMode]
    public class AudiophilePlayer : MonoBehaviour
    {
        public string Id => id;
        private string id;

        public bool loop;
        public AudioSource audioSource;

        bool isPlaying = false;
        public bool IsPlaying => isPlaying;

        SoundEventData seData = null;
        private float? overrideVolume = null;
        private float? overridePitch = null;

        public event Action onStopped;
        public event Action onLooped;


        public void Awake()
        {
            audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += Update;
#endif
        }

        public void Play(SoundEventData soundEventData, string id)
        {
            seData = soundEventData;
            this.loop = soundEventData.Loop;
            audioSource.loop = false;

            this.id = id;

            audioSource.volume = overrideVolume ?? UnityEngine.Random.Range(soundEventData.StandardSettings.MinVolume, soundEventData.StandardSettings.MaxVolume);
            audioSource.pitch = overridePitch ?? UnityEngine.Random.Range(soundEventData.StandardSettings.MinPitch, soundEventData.StandardSettings.MaxPitch);
            audioSource.outputAudioMixerGroup = soundEventData.StandardSettings.Group;

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
            if(isPlaying)
            {
                if(this.audioSource != null && !this.audioSource.isPlaying)
                {
                    if (loop)
                    {
                        onLooped?.Invoke();
                        Play(seData, this.id);
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

            this.onStopped?.Invoke();
            this.onStopped = null;
            this.onLooped = null;
        }

        internal void SetOverrideVolume(float? volume)
        {
            if(volume != null)
            {
                this.overrideVolume = volume;
                this.audioSource.volume = volume.Value;

            }
        }

        internal void SetOverridePitch(float? pitch)
        {
            if (pitch != null)
            {
                this.overridePitch = pitch;
                this.audioSource.pitch = pitch.Value;
            }
        }
    }
}
