using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelDust.Audiophile
{
    public class AudiophilePlayResult
    {
        private AudiophilePlayer audiophilePlayer;
        private AudiophilePlayer AudiophilePlayer => audiophilePlayer;

        public AudiophilePlayResult(AudiophilePlayer audiophilePlayer)
        {
            this.audiophilePlayer = audiophilePlayer;
            this.audiophilePlayer.onStopped += OnStopped;
        }

        private void OnStopped()
        {
            Debug.Log("Stopped");
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

        public void SetPitch(float pitch)
        {
            if (this.audiophilePlayer)
            {
                this.audiophilePlayer.audioSource.pitch = pitch;
            }
            else
            {
                Debug.Log("Audiophile - Audiophile player is null");
            }
        }

        public void SetVolume(float volume)
        {
            if(this.audiophilePlayer)
            {
                this.audiophilePlayer.audioSource.volume = volume;
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
        public event Action onStopped;

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
            if(isPlaying)
            {
                if(!this.audioSource.isPlaying)
                {
                    if (loop)
                    {
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
        }
    }
}
