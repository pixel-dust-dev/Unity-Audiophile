using UnityEditor;

namespace PixelDust.Audiophile
{
    public class AudiophileProjectSettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateAudiophileSettingsProvider()
        {
            var settings = new SerializedObject(AudiophileProjectSettings.Instance);
            var editor = Editor.CreateEditor(settings.targetObject);

            var provider = new SettingsProvider("Audiophile/AudiophileProjectSettings", SettingsScope.Project)
            {
                label = "Settings",
                guiHandler = (searchContext) =>
                {
                    editor.OnInspectorGUI();
                }
            };

            return provider;
        }
    }
}