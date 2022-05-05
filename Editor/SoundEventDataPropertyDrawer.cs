using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PixelDust.Audiophile
{
    [CustomPropertyDrawer(typeof(SoundEventData))]
    public class SoundEventDataPropertyDrawer : PropertyDrawer
    {
        public static GUIStyle PresetStyle()
        {
            GUIStyle presetStyle = new GUIStyle();
            presetStyle.normal.background = (Texture2D)EditorGUIUtility.IconContent("d_Preset.Context@2x").image;
            return presetStyle;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;

            height += 5 * 5;
            height += ExtraEditorGUIUtility.SingleLineHeight() * 1;
            var audioClipsProp = property.FindPropertyRelative("audioClips");
            if (audioClipsProp != null)
            {
                height += EditorGUI.GetPropertyHeight(audioClipsProp, true);
            }

            if (property.serializedObject.targetObject is SoundEventPreset)
            {
                height += ExtraEditorGUIUtility.SingleLineHeight();
            }

            //Basic Settings
            height += ExtraEditorGUIUtility.SingleLineHeight() * 4;

            //Advanced
            var advancedSettingsProp = property.FindPropertyRelative("advancedSettings");
            float advancedSettingsHeight = EditorGUI.GetPropertyHeight(advancedSettingsProp);
            height += advancedSettingsHeight;

            //Spatial
            var spatialSettingsProp = property.FindPropertyRelative("spatialSettings");
            float spatialSettingsHeight = EditorGUI.GetPropertyHeight(spatialSettingsProp);
            height += spatialSettingsHeight;

            height += EditorGUIUtility.standardVerticalSpacing;
            return height;
        }

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            SoundEventData soundEventData;
            if (!PropertyDrawerUtility.GetTargetObjectOfProperty<SoundEventData>(property, out soundEventData))
            {
                Debug.LogError("There was a problem.");
                return;
            }

            Rect position = pos;
            EditorGUI.BeginProperty(position, label, property);

            DrawData(ref position, property, soundEventData);

            EditorGUI.EndProperty();
        }

        private static void DrawData(ref Rect position, SerializedProperty data, SoundEventData soundEventData)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            if(data.serializedObject.targetObject is SoundEventPreset)
            {
                Rect playStopRect = position;
                SoundEventPropertyDrawer.DrawPlayStop(playStopRect, data);
                position.y += ExtraEditorGUIUtility.SingleLineHeight();
            }

            position.y += 5;
            position = DrawClips(position, data);
            position.y += 5;
            position = DrawBasicSettingsSection(position, data);
            position.y += 5;
            position = DrawSpatialSettingsSection(position, data);
            position.y += 5;
            position = DrawAdvancedSettingsSection(position, data);
            position.y += 5;
            position = DrawSoundId(position, data);
        }

        private static Rect DrawAdvancedSettingsSection(Rect position, SerializedProperty data)
        {
            var advancedSettingsProp = data.FindPropertyRelative("advancedSettings");
            EditorGUI.PropertyField(position, advancedSettingsProp);
            float height = EditorGUI.GetPropertyHeight(advancedSettingsProp);
            position.y += height;

            return position;
        }

        private static Rect DrawSpatialSettingsSection(Rect position, SerializedProperty data)
        {
            var spatialSettingsProp = data.FindPropertyRelative("spatialSettings");
            EditorGUI.PropertyField(position, spatialSettingsProp);
            float height = EditorGUI.GetPropertyHeight(spatialSettingsProp);
            position.y += height;

            return position;
        }

        private static Rect DrawSoundId(Rect position, SerializedProperty data)
        {
            var soundIdProp = data.FindPropertyRelative("soundId");
            if (string.IsNullOrEmpty(soundIdProp.stringValue))
            {
                soundIdProp.stringValue = GUID.Generate().ToString();
            }
            EditorGUI.PropertyField(position, soundIdProp, true);
            var propHeight = EditorGUI.GetPropertyHeight(soundIdProp, true);
            position.y += propHeight + EditorGUIUtility.standardVerticalSpacing;
            return position;
        }

        private static Rect DrawBasicSettingsSection(Rect position, SerializedProperty data)
        {
            var standardSettingsProp = data.FindPropertyRelative("standardSettings");

            Rect basicSectionPos = position;
            var minVolumeProp = standardSettingsProp.FindPropertyRelative("minVolume");
            var maxVolumeProp = standardSettingsProp.FindPropertyRelative("maxVolume");
            DrawRangeSlider(ref position, new GUIContent("Volume"), minVolumeProp, maxVolumeProp, 0, 1, AudiophileProjectSettings.VolDisplayMode);

            var minPitchProp = standardSettingsProp.FindPropertyRelative("minPitch");
            var maxPitchProp = standardSettingsProp.FindPropertyRelative("maxPitch");
            DrawRangeSlider(ref position, new GUIContent("Pitch"), minPitchProp, maxPitchProp, -3, 3, AudiophileProjectSettings.Units.Linear);

            {
                var groupProp = standardSettingsProp.FindPropertyRelative("group");
                EditorGUI.PropertyField(position, groupProp);
                var propHeight = EditorGUI.GetPropertyHeight(groupProp, true);
                position.y += propHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            //var groups = AudiophileEditor.AudiophileEditorUtility.GetMixerGroups();
            //{
            //    var groupProp = standardSettingsProp.FindPropertyRelative("group");

            //    var found = groups.FirstOrDefault(x => x.name == groupProp.stringValue);

            //    if(groupProp.hasMultipleDifferentValues)
            //    {
            //        EditorGUI.showMixedValue = true;
            //    }
            //    int currIndex = found ? groups.IndexOf(found) : 0;
            //    int newIndex = EditorGUI.Popup(position, groupProp.displayName, currIndex, groups.Select(x => $"{x.name}").ToArray());
            //    string val = groups[newIndex].name;
            //    groupProp.stringValue = groups[newIndex].name;

            //    var propHeight = EditorGUI.GetPropertyHeight(groupProp, true);
            //    position.y += propHeight + EditorGUIUtility.standardVerticalSpacing;
            //    EditorGUI.showMixedValue = false;
            //}

            {
                var loopProp = data.FindPropertyRelative("loop");
                EditorGUI.PropertyField(position, loopProp, true);
                var propHeight = EditorGUI.GetPropertyHeight(loopProp, true);
                position.y += propHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            return position;
        }

        private static Rect DrawClips(Rect position, SerializedProperty data)
        {
            var audioClipsProp = data.FindPropertyRelative("audioClips");

            Rect clipsPos = position;

            EditorGUI.PropertyField(clipsPos, audioClipsProp, new GUIContent("Clips"), true);
            var propHeight = EditorGUI.GetPropertyHeight(audioClipsProp, true);
            position.y += propHeight + EditorGUIUtility.standardVerticalSpacing;
            return position;
        }

        internal static float DrawRangeSlider(ref Rect position, GUIContent label, SerializedProperty minProp, SerializedProperty maxProp, float min, float max, AudiophileProjectSettings.Units units)
        {
            var height = 0f;
            var minValue = minProp.floatValue;
            var maxValue = maxProp.floatValue;
            int floatFieldWidth = 40;
            int padding = 0;

            Rect labelRect = new Rect(position);
            labelRect.width = EditorGUIUtility.labelWidth;

            if(units == AudiophileProjectSettings.Units.Decibels)
            {
                label.text += " (dB)";
            }

            EditorGUI.LabelField(labelRect, label);

            Rect propertyRect = new Rect(position);
            propertyRect.x += EditorGUIUtility.labelWidth;
            propertyRect.width -= EditorGUIUtility.labelWidth;

            Rect sliderRect = new Rect(propertyRect.x + (floatFieldWidth + padding), propertyRect.y, propertyRect.width - (floatFieldWidth + padding) * 2, propertyRect.height);

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.MinMaxSlider(sliderRect, ref minValue, ref maxValue, min, max);
            }
            if (EditorGUI.EndChangeCheck())
            {
                minValue = (float)System.Math.Round(minValue, 3);
                maxValue = (float)System.Math.Round(maxValue, 3);
            }
            if (units == AudiophileProjectSettings.Units.Linear)
            {
                Rect minFieldRect = new Rect(propertyRect.x, propertyRect.y, floatFieldWidth, propertyRect.height);
                EditorGUI.showMixedValue = minProp.hasMultipleDifferentValues;
                minValue = EditorGUI.FloatField(minFieldRect, minValue, EditorStyles.miniBoldLabel);
                EditorGUI.showMixedValue = false;

                EditorGUI.showMixedValue = maxProp.hasMultipleDifferentValues;
                Rect maxFieldRect = new Rect((propertyRect.x + propertyRect.width) - (floatFieldWidth), propertyRect.y, floatFieldWidth, propertyRect.height);
                maxValue = EditorGUI.FloatField(maxFieldRect, maxValue, EditorStyles.miniBoldLabel);
                EditorGUI.showMixedValue = false;
            }
            else if(units == AudiophileProjectSettings.Units.Decibels)
            {
                Rect minFieldRect = new Rect(propertyRect.x, propertyRect.y, floatFieldWidth, propertyRect.height);
                minValue = AudioHelper.DecibelToLinear(EditorGUI.FloatField(minFieldRect, AudioHelper.LinearToDecibel(minValue), EditorStyles.miniBoldLabel));
                
                Rect maxFieldRect = new Rect((propertyRect.x + propertyRect.width) - (floatFieldWidth), propertyRect.y, floatFieldWidth, propertyRect.height);
                maxValue = AudioHelper.DecibelToLinear(EditorGUI.FloatField(maxFieldRect, AudioHelper.LinearToDecibel(maxValue), EditorStyles.miniBoldLabel));
            }

            if (minValue != minProp.floatValue)
            {
                minProp.floatValue = minValue;
            }
            if(maxValue != maxProp.floatValue)
            {
                maxProp.floatValue = maxValue;
            }

            position.y += ExtraEditorGUIUtility.SingleLineHeight();
            height += ExtraEditorGUIUtility.SingleLineHeight();
            return height;
        }
    }
}