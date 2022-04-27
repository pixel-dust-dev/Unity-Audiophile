using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace PixelDust.Audiophile
{
    public class SoundManager : MonoBehaviour
    {
        #region Singleton
        static SoundManager instance;
        public static SoundManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = CreateInstance();
                    instance.Init();
                }
                return instance;
            }
        }

        private static SoundManager CreateInstance()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return CreateInstance_RUNTIME();
            }
            else
            {
                return CreateInstance_EDITOR();
            }
#endif
            #pragma warning disable
            return CreateInstance_RUNTIME();

            SoundManager CreateInstance_RUNTIME()
            {
                SoundManager soundManager = new GameObject("SoundManager").AddComponent<SoundManager>();
                soundManager.poolSize = AudiophileProjectSettings.PoolSize;
                DontDestroyOnLoad(soundManager);
                return soundManager;
            }
#if UNITY_EDITOR
            SoundManager CreateInstance_EDITOR()
            {
                var existingSoundManager = Resources.FindObjectsOfTypeAll<SoundManager>().FirstOrDefault();
                if (existingSoundManager != null)
                {
                    DestroyImmediate(existingSoundManager.gameObject);
                }

                SoundManager soundManager;
                soundManager = new GameObject("SoundManager").AddComponent<SoundManager>();
                soundManager.poolSize = AudiophileProjectSettings.PoolSize;
                soundManager.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.NotEditable;

                UnityEditor.EditorApplication.playModeStateChanged += ModeStateChanged;

                return soundManager;
            }

            void ModeStateChanged(PlayModeStateChange obj)
            {
                if (obj == PlayModeStateChange.ExitingEditMode)
                {
                    DestroyImmediate(SoundManager.Instance.gameObject);
                    UnityEditor.EditorApplication.playModeStateChanged -= ModeStateChanged;
                }
            }
#endif
        }
        #endregion

        #region Public
        public void TransitionToSnapshots(AudioMixerSnapshot[] snapShots, float[] weights, float timeToReach)
        {
            if (Mixer == null) { Debug.LogError("No Mixer", Instance); return; }

            Mixer.TransitionToSnapshots(snapShots, weights, timeToReach);
        }

        /// <summary>
        /// Stops a sound based on an Id.
        /// </summary>
        /// <param name="id">The Id of the sound</param>
        public static void StopSound(string id)
        {
            if (playingAudioSources.ContainsKey(id))
            {
                foreach (var audioSource in playingAudioSources[id])
                {
                    audioSource.Stop();
                }
                playingAudioSources.Remove(id);
            }
        }

        /// <summary>
        /// Processes a list of sound events. Will play every single sound event.
        /// </summary>
        /// <param name="soundEvents"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static List<string> ProcessSounds(IEnumerable<SoundEvent> soundEvents, Vector3 position)
        {
            List<string> idList = new List<string>();

            var enumerator = soundEvents.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var id = ProcessSound(enumerator.Current, position);
                if (id != null)
                {
                    idList.Add(id);
                }
            }

            return idList;
        }

        /// <summary>
        /// Processes a single sound event.
        /// </summary>
        /// <param name="soundEvent"></param>
        /// <param name="position"></param>
        /// <param name="overrideId"></param>
        /// <returns></returns>
        public static string ProcessSound(SoundEvent soundEvent, Vector3? position = null, string overrideId = null)
        {
            return ProcessSound(soundEvent.Data, position, overrideId);
        }

        public static string ProcessSound(SoundEventData soundEventData, Vector3? position = null, string overrideId = null)
        {
            var id = overrideId;
            if (string.IsNullOrEmpty(id))
            {
                id = soundEventData.SoundId;
            }
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }

            AudioSource audioSource;
            audioSource = GetAudioSource();
            if (audioSource == null) { Debug.Log("Pool overflowed."); return null; }

            audioSource.transform.position = position.HasValue ? position.Value : Vector3.zero;
            audioSource.volume = Random.Range(soundEventData.StandardSettings.MinVolume, soundEventData.StandardSettings.MaxVolume);
            audioSource.pitch = Random.Range(soundEventData.StandardSettings.MinPitch, soundEventData.StandardSettings.MaxPitch);
            audioSource.outputAudioMixerGroup = GetGroup(soundEventData.StandardSettings.Group);
            audioSource.loop = soundEventData.Loop;
            //int index = Random.Range(0, soundEventData.AudioClips.Length);

            //Spatial Type
            audioSource.spatialBlend = soundEventData.SpatialSettings.Is3D ? 1 : 0;

            //3D
            audioSource.dopplerLevel = soundEventData.SpatialSettings.DopplerLevel;
            audioSource.spread = soundEventData.SpatialSettings.Spread;
            audioSource.rolloffMode = soundEventData.SpatialSettings.VolumeRolloff;
            audioSource.minDistance = soundEventData.SpatialSettings.MinDistance;
            audioSource.maxDistance = soundEventData.SpatialSettings.MaxDistance;

            //2D
            audioSource.panStereo = Random.Range(soundEventData.SpatialSettings.StereoPanMin, soundEventData.SpatialSettings.StereoPanMax);

            //Advanced
            audioSource.bypassEffects = soundEventData.AdvancedSettings.BypassEffects;
            audioSource.bypassListenerEffects = soundEventData.AdvancedSettings.BypassListenerEffects;
            audioSource.bypassReverbZones = soundEventData.AdvancedSettings.BypassReverbZones;
            audioSource.reverbZoneMix = soundEventData.AdvancedSettings.ReverbZoneMix;

            AudioClip clip = soundEventData.AudioClips.GetRandom();

            audioSource.clip = clip;

            if (clip != null)
            {
                audioSource.gameObject.name = clip.name + " - vol: " + audioSource.volume + " - pitch: " + audioSource.pitch;
                audioSource.Play();
            }

            if (!playingAudioSources.ContainsKey(id))
            {
                playingAudioSources.Add(id, new List<AudioSource>());
            }
            playingAudioSources[id].Add(audioSource);

            return id;
        }
        #endregion

        #region Private
        private static AudioMixer Mixer => Audiophile.AudiophileProjectSettings.AudioMixer;

        
        private AudioSource[] audioSources;
        // Collection checks will throw errors if we try to release an item that is already in the pool.
        
        [SerializeField]
        private int poolSize = 10;

        private AudioSource musicSource;

        private AudioSource ambientSource;

        private static Dictionary<string, List<AudioSource>> playingAudioSources = new Dictionary<string, List<AudioSource>>();
        
        private static AudioMixerGroup GetGroup(string group)
        {
            if (SoundManager.Mixer == null) { Debug.Log("NO MIXER"); return null; }
            return GetFirstGroup(group);

            AudioMixerGroup GetFirstGroup(string name)
            {
                var matchingGroups = SoundManager.Mixer.FindMatchingGroups(name);
                if (matchingGroups == null || matchingGroups.Length == 0) { return null; }
                return matchingGroups[0];
            }
        }

        private void Init()
        {
            //Music
            musicSource = MakeSource("MusicAudioSource");
            musicSource.loop = true;

            //Ambient
            ambientSource = MakeSource("AmbientAudioSource");
            ambientSource.loop = true;

            audioSources = new AudioSource[poolSize];

            for (int i = 0; i < poolSize; i++)
            {
                audioSources[i] = MakeSource($"AudioSource_{i}");
            }
        }

        private AudioSource MakeSource(string name)
        {
            AudioSource newSource = new GameObject(name).AddComponent<AudioSource>();

            newSource.transform.SetParent(this.transform);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                newSource.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.NotEditable;
            }
#endif
            return newSource;
        }

        private static AudioSource GetAudioSource()
        {
            for (int i = 0; i < Instance.audioSources.Length; i++)
            {
                if (!Instance.audioSources[i].isPlaying)
                {
                    foreach (var item in playingAudioSources.Where(kvp => kvp.Value.Contains(Instance.audioSources[i])).ToList())
                    {
                        playingAudioSources[item.Key].Remove(Instance.audioSources[i]);
                        if (playingAudioSources[item.Key].Count == 0)
                        {
                            playingAudioSources.Remove(item.Key);
                        }
                    }
                    return Instance.audioSources[i];
                }
            }
            return null;
        }
        #endregion

        #region TODO
        //TODO:
        //public static void ChangeMusicTrack(MusicTrackChangeEvent musicTrackChangeEvent)
        //{
        //    if(musicTrackChangeEvent == null)
        //    {
        //        Instance.musicSource.Stop();
        //        return;
        //    }

        //    if(Instance.musicSource.clip != musicTrackChangeEvent.musicClip)
        //    {
        //        Instance.musicSource.clip = musicTrackChangeEvent.musicClip;
        //        Instance.musicSource.Play();
        //    }

        //    Instance.musicSource.volume = musicTrackChangeEvent.volume;
        //    Instance.musicSource.pitch = musicTrackChangeEvent.pitch;
        //    //Instance.musicSource.outputAudioMixerGroup = GetGroup(Audiophile.AudiophileProjectSettings.MusicMixerGroup);
        //}

        //public static void ChangeAmbientTrack(AmbientTrackChangeEvent ambientTrackChangeEvent)
        //{
        //    if (ambientTrackChangeEvent == null)
        //    {
        //        Instance.ambientSource.Stop();
        //        return;
        //    }

        //    if (Instance.ambientSource.clip != ambientTrackChangeEvent.AmbientClip)
        //    {
        //        Instance.ambientSource.clip = ambientTrackChangeEvent.AmbientClip;
        //        Instance.ambientSource.Play();
        //    }

        //    Instance.ambientSource.volume = ambientTrackChangeEvent.Volume;
        //    Instance.ambientSource.pitch = ambientTrackChangeEvent.Pitch;
        //    Instance.ambientSource.outputAudioMixerGroup = GetGroup(ambientTrackChangeEvent.MixerGroup);
        //    //Instance.ambientSource.outputAudioMixerGroup = GetGroup(SoundGroup.Ambient);
        //}
        #endregion
    }
}