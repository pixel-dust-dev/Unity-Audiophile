using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelDust.Audiophile
{
    [ExecuteInEditMode]
    public class AudiophilePlayer : MonoBehaviour
    {
        public string Id => id;
        private string id;

        public bool loop;
        public AudioSource AudioSource => audioSource;
        internal AudioSource audioSource;

        bool isPlaying = false;
        public bool IsPlaying => isPlaying;


        SoundEventData seData = null;
        private float? overrideVolume = null;
        private float? overridePitch = null;
        private bool? overrideLoop = null;
        public bool Persist = false;
        public Transform FollowTransform = null;

        public event Action onStopped;
        public event Action onLooped;


        public void Awake()
        {
            audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += Update;
#endif
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ActiveSceneChanged;
        }



        private void ActiveSceneChanged(Scene scene1, Scene scene2)
        {
            if(!Persist)
            {
                this.Stop();
            }
        }

        public void Play(SoundEventData soundEventData, float delay, string id)
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
                audioSource.PlayDelayed(delay);
                this.isPlaying = true;
            }
        }

        public void Update()
        {
            if (this == null) { return; }
            if (!this.enabled) { return; }

            if(this.FollowTransform != null)
            {
                this.transform.position = this.FollowTransform.position;
            }

            if (isPlaying)
            {
                if(this.audioSource != null && !this.audioSource.isPlaying)
                {
                    if (loop)
                    {
                        onLooped?.Invoke();
                        Play(seData, 0, this.id);
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

            this.SetOverrideVolume(null);
            this.SetOverridePitch(null);
            this.SetPersist(false);
            this.SetFollowTransform(null);

            SoundManager.RemoveAudiophilePlayer(this.id, this);
        }

        internal void SetOverrideLoop(bool? loop)
        {
            this.overrideLoop = loop;
            if(loop != null)
            {
                this.loop = loop.Value;
            }
        }

        internal void SetOverrideVolume(float? volume)
        {
            this.overrideVolume = volume;
            if(volume != null)
            {
                this.audioSource.volume = volume.Value;
            }
        }

        internal void SetOverridePitch(float? pitch)
        {
            this.overridePitch = pitch;
            if (pitch != null)
            {
                this.audioSource.pitch = pitch.Value;
            }
        }

        internal void SetPersist(bool value)
        {
            this.Persist = value;
        }

        internal void SetFollowTransform(Transform followTransform)
        {
            this.FollowTransform = followTransform;
        }
    }
}
