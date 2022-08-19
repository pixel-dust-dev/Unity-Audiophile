using System;
using UnityEngine;
using UnityEngine.Audio;

namespace PixelDust.Audiophile
{
    public partial class AudiophileProjectSettings : ScriptableObject
    {
        #region Settings
        [Header("Audio Pool")]
        [SerializeField]
        private int poolSize = 30;
        public static int PoolSize => Instance.poolSize;

        public enum Units { Linear, Decibels }
        [Header("Units")]
        [SerializeField]
        private Units volDisplayMode;
        public static Units VolDisplayMode => Instance.volDisplayMode;

        [SerializeField]
        private AudioMixer audioMixer;
        public static AudioMixer AudioMixer => Instance.audioMixer;

        [Header("Defaults")]
        [SerializeField]
        private SpatialSettingsDefault[] spatialSettings;
        public static SpatialSettingsDefault[] SpatialSettings => Instance.spatialSettings;

        [SerializeField]
        private AdvancedSettingsDefault[] advancedSettings;
        public static AdvancedSettingsDefault[] AdvancedSettings => Instance.advancedSettings;

        #endregion

        #region Creation
#if UNITY_EDITOR
        public static string FullPath
        {
            get
            {
                return $"Assets/Resources/{Name}.asset";
            }
        }
#endif

        public static string Name
        {
            get
            {
                return $"{nameof(AudiophileProjectSettings)}";
            }
        }

        private static AudiophileProjectSettings instance;
        public static AudiophileProjectSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (AudiophileProjectSettings)Resources.Load(Name, typeof(AudiophileProjectSettings));

                    if (instance == null)
                    {
                        Debug.LogError("Audiophile: No Settings Exist");
#if UNITY_EDITOR
                        if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources"))
                        {
                            UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
                        }
                        instance = CreateInstance<AudiophileProjectSettings>();
                        UnityEditor.AssetDatabase.CreateAsset(instance, FullPath); 
                        Debug.Log("Created Settings: " + FullPath);
                        UnityEditor.AssetDatabase.SaveAssets();
#endif
                    }
                }
                return instance;
            }
        }
#endregion
    }

    [System.Serializable]
    public class SpatialSettingsDefault
    {
        [SerializeField]
        public string name;
        public string Name => name;

        [SerializeField]
        private SpatialSettings data;
        public SpatialSettings Data => data;
    }

    [System.Serializable]
    public class AdvancedSettingsDefault
    {
        [SerializeField]
        private string name;
        public string Name => name;

        [SerializeField]
        private AdvancedSettings data;
        public AdvancedSettings Data => data;
    }
}