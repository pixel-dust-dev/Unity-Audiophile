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
            if (playingAudioPhilePlayers.ContainsKey(id))
            {
                foreach (var audioSource in playingAudioPhilePlayers[id])
                {
                    audioSource.Stop();
                }
                playingAudioPhilePlayers.Remove(id);
            }
        }

        /// <summary>
        /// Processes a list of sound events. Will play every single sound event.
        /// </summary>
        /// <param name="soundEvents"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static List<AudiophilePlayResult> ProcessSounds(IEnumerable<SoundEvent> soundEvents, Vector3 position)
        {
            List<AudiophilePlayResult> audiophilePlayResults = new List<AudiophilePlayResult>();

            var enumerator = soundEvents.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var id = ProcessSound(enumerator.Current, position);
                if (id != null)
                {
                    audiophilePlayResults.Add(id);
                }
            }

            return audiophilePlayResults;
        }

        /// <summary>
        /// Processes a single sound event.
        /// </summary>
        /// <param name="soundEvent"></param>
        /// <param name="position"></param>
        /// <param name="overrideId"></param>
        /// <returns></returns>
        public static AudiophilePlayResult ProcessSound(SoundEvent soundEvent, Vector3? position = null, float delay = 0, string overrideId = null)
        {
            return ProcessSound(soundEvent.Data, position, delay, overrideId);
        }

        public static AudiophilePlayResult ProcessSound(SoundEventData soundEventData, Vector3? position = null, float delay = 0, string overrideId = null)
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

            AudiophilePlayer audiophilePlayer;
            audiophilePlayer = GetAudioPhilePlayer();
            if (audiophilePlayer == null) { Debug.Log("Pool overflowed."); return null; }

            audiophilePlayer.transform.position = position.HasValue ? position.Value : Vector3.zero;

            audiophilePlayer.Play(soundEventData, delay, id);

            if (!playingAudioPhilePlayers.ContainsKey(id))
            {
                playingAudioPhilePlayers.Add(id, new List<AudiophilePlayer>());
            }
            playingAudioPhilePlayers[id].Add(audiophilePlayer);

            return new AudiophilePlayResult(audiophilePlayer);
        }
        #endregion

        #region Private
        private static AudioMixer Mixer => Audiophile.AudiophileProjectSettings.AudioMixer;

        
        private AudiophilePlayer[] audiophilePlayers;
        // Collection checks will throw errors if we try to release an item that is already in the pool.
        
        [SerializeField]
        private int poolSize = 10;

        private AudiophilePlayer musicSource;

        private AudiophilePlayer ambientSource;

        private static Dictionary<string, List<AudiophilePlayer>> playingAudioPhilePlayers = new Dictionary<string, List<AudiophilePlayer>>();
        
        public static AudioMixerGroup GetMixerGroup(string group)
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

            audiophilePlayers = new AudiophilePlayer[poolSize];

            for (int i = 0; i < poolSize; i++)
            {
                audiophilePlayers[i] = MakeSource($"AudioSource_{i}");
            }
        }

        private AudiophilePlayer MakeSource(string name)
        {
            AudiophilePlayer newSource = new GameObject(name).AddComponent<AudiophilePlayer>();

            newSource.transform.SetParent(this.transform);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                newSource.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.NotEditable;
            }
#endif
            return newSource;
        }

        private static AudiophilePlayer GetAudioPhilePlayer()
        {
            for (int i = 0; i < Instance.audiophilePlayers.Length; i++)
            {
                if (!Instance.audiophilePlayers[i].IsPlaying)
                {
                    foreach (var item in playingAudioPhilePlayers.Where(kvp => kvp.Value.Contains(Instance.audiophilePlayers[i])).ToList())
                    {
                        playingAudioPhilePlayers[item.Key].Remove(Instance.audiophilePlayers[i]);
                        if (playingAudioPhilePlayers[item.Key].Count == 0)
                        {
                            playingAudioPhilePlayers.Remove(item.Key);
                        }
                    }
                    return Instance.audiophilePlayers[i];
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