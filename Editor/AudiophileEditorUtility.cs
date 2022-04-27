using PixelDust.Audiophile;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace PixelDust.AudiophileEditor
{
    public class AudiophileEditorUtility
    {
        #region Sound Event
        [MenuItem("Assets/Audiophile/Create Sound Event Preset", priority = 1)]
        public static void CreateSoundEventFromClips()
        {
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                if (Selection.objects[i].GetType() == typeof(AudioClip))
                {
                    string path = GetActivePath(Selection.objects[i]);
                    AudioClip[] clips = Selection.objects.OfType<AudioClip>().ToArray();
                    SoundEventPreset soundEventPreset = CreateSoundEventFromAudioClips(path, clips);

                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = soundEventPreset;

                    break;
                }
            }
        }

        [MenuItem("Assets/Audiophile/Create Sound Event Preset", priority = 1, validate = true)]
        public static bool CreateSoundEventFromClipsValidate()
        {
            bool hasAnimationClip = false;
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                if (Selection.objects[i].GetType() == typeof(AudioClip))
                {
                    hasAnimationClip = true;
                    break;
                }
            }
            return hasAnimationClip;
        }

        public static SoundEventPreset CreateSoundEventFromAudioClips(string path, AudioClip[] clips)
        {
            SoundEventPreset soundEventPreset = CreateMyAsset<SoundEventPreset>(path, clips[0].name);
            SerializedObject soundEventPresetSO = new SerializedObject(soundEventPreset);
            //TODO: Set audio clips
            //moveSO.FindProperty("animationClip").objectReferenceValue = clip;
            var audioClipsProp = soundEventPresetSO.FindProperty("data").FindPropertyRelative("audioClips").FindPropertyRelative("weightedObjects");
            audioClipsProp.arraySize = clips.Length;
            for (int i = 0; i < audioClipsProp.arraySize; i++)
            {
                var arrayElement = audioClipsProp.GetArrayElementAtIndex(i);
                var contentProp = arrayElement.FindPropertyRelative("contents");
                contentProp.objectReferenceValue = clips[i];
                var weightProp = arrayElement.FindPropertyRelative("weight");
                weightProp.floatValue = 1;
            }
            soundEventPresetSO.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();

            return soundEventPreset;
        }

        #endregion

        #region
        [MenuItem("Assets/Audiophile/Create Sound Event Collection Preset", priority = 1)]
        public static void CreateCollectionFromSoundEvents()
        {
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                if (Selection.objects[i].GetType() == typeof(SoundEventPreset))
                {
                    string path = GetActivePath(Selection.objects[i]);
                    SoundEventPreset[] soundEventPresets = Selection.objects.OfType<SoundEventPreset>().ToArray();
                    SoundEventCollectionPreset soundEventCollectionPreset = CreateCollectionFromSoundEvents(path, soundEventPresets);

                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = soundEventCollectionPreset;

                    break;
                }
            }
        }

        [MenuItem("Assets/Audiophile/Create Sound Event Collection Preset", priority = 1, validate = true)]
        public static bool CreateCollectionFromSoundEventsValidate()
        {
            bool hasAnimationClip = false;
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                if (Selection.objects[i].GetType() == typeof(SoundEventPreset))
                {
                    hasAnimationClip = true;
                    break;
                }
            }
            return hasAnimationClip;
        }

        public static SoundEventCollectionPreset CreateCollectionFromSoundEvents(string path, SoundEventPreset[] soundEvents)
        {
            SoundEventCollectionPreset soundEventCollectionPreset = CreateMyAsset<SoundEventCollectionPreset>(path, soundEvents[0].name);
            SerializedObject soundEventCollectionPresetSO = new SerializedObject(soundEventCollectionPreset);
            //TODO: Set audio clips
            //moveSO.FindProperty("animationClip").objectReferenceValue = clip;
            var soundEventsProp = soundEventCollectionPresetSO.FindProperty("data").FindPropertyRelative("soundEventCollection").FindPropertyRelative("weightedObjects");
            soundEventsProp.arraySize = soundEvents.Length;
            for (int i = 0; i < soundEventsProp.arraySize; i++)
            {
                var arrayElement = soundEventsProp.GetArrayElementAtIndex(i);
                var contentProp = arrayElement.FindPropertyRelative("contents");
                var preset = contentProp.FindPropertyRelative("soundEventPreset");
                preset.objectReferenceValue = soundEvents[i];
                var weightProp = arrayElement.FindPropertyRelative("weight");
                weightProp.floatValue = 1;
            }
            soundEventCollectionPresetSO.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();

            return soundEventCollectionPreset;
        }
        #endregion

        #region Utility

        public static T CreateMyAsset<T>(string folderPath, string assetName, bool replace = false) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            if (!string.IsNullOrEmpty(folderPath))
            {
                if (!CreateMissingFolders(folderPath))
                {
                    Debug.LogError("Folders could not be created");
                    return null;
                }
            }
            else
            {
                folderPath = "Assets";
            }

            string name = folderPath + "/" + assetName + ".asset";
            if (!replace) { name = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + assetName + ".asset"); }
            else
            {
                AssetDatabase.DeleteAsset(name);
            }
            AssetDatabase.CreateAsset(asset, name);
            AssetDatabase.SaveAssets();

            return asset;
        }

        private static bool CreateMissingFolders(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder))
            {
                return true;
            }

            var split = folder.Split('/');

            string prevFolder = "";
            for (int i = 0; i < split.Length; i++)
            {
                string fullPath = "";
                if (prevFolder.Length > 0) { fullPath += "/"; }
                fullPath += split[i];

                if (!AssetDatabase.IsValidFolder(fullPath))
                {
                    if (string.IsNullOrEmpty(AssetDatabase.CreateFolder(prevFolder, split[i])))
                    {
                        return false;
                    }
                }

                if (prevFolder.Length > 0) { prevFolder += "/"; }
                prevFolder += split[i];
            }

            return true;
        }

        public static string GetActivePath(Object activeObject)
        {
            string path = "";
            if (activeObject != null)
            {
                path = AssetDatabase.GetAssetPath(activeObject.GetInstanceID());
                if (!AssetDatabase.IsValidFolder(path))
                {
                    path = System.IO.Path.GetDirectoryName(path);
                }
            }

            return path;
        }

        #endregion
        public static List<AudioMixerGroup> GetMixerGroups()
        {
            var assetPath = AssetDatabase.GetAssetPath(AudiophileProjectSettings.AudioMixer);
            var assetsAtPath = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);

            List<AudioMixerGroup> groups = new List<AudioMixerGroup>();

            foreach (var assetRepresentation in assetsAtPath)
            {
                //AudioMixerSnapshot snapshot = assetRepresentation as AudioMixerSnapshot;
                //if (snapshot != null)
                //{
                //    Debug.Log($"mixer snapshots: {snapshot.name}");
                //}

                AudioMixerGroup mixerGroup = assetRepresentation as AudioMixerGroup;
                if (mixerGroup != null)
                {
                    groups.Add(mixerGroup);
                }
            }

            return groups;
        }
    }
}
