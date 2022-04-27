using UnityEditor;
using UnityEngine;

namespace PixelDust.Audiophile
{
    [CustomPropertyDrawer(typeof(AdvancedSettings))]
    public class AdvancedSettingsDrawer : PropertyDrawer
    {
        const string ADV_SETTINGS_SECTION_FOLDOUT_KEY = "PixelDust.Audiophile.SoundEventDataPropertyDrawer.AdvancedSettingsSectionFoldout";
        static bool AdvancedSettingsSectionFoldout
        {
            get
            {
                return EditorPrefs.GetBool(ADV_SETTINGS_SECTION_FOLDOUT_KEY);
            }
            set
            {
                EditorPrefs.SetBool(ADV_SETTINGS_SECTION_FOLDOUT_KEY, value);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;
            height += ExtraEditorGUIUtility.SingleLineHeight();
            if (AdvancedSettingsSectionFoldout)
            {
                height += ExtraEditorGUIUtility.SingleLineHeight() * 3;
                var bypassReverbZonesProp = property.FindPropertyRelative("bypassReverbZones");
                if (!bypassReverbZonesProp.boolValue)
                {
                    height += ExtraEditorGUIUtility.SingleLineHeight();
                }
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            Rect threeDSectionRect = EditorGUI.IndentedRect(position);
            AdvancedSettingsSectionFoldout = EditorGUI.BeginFoldoutHeaderGroup(
                threeDSectionRect, 
                AdvancedSettingsSectionFoldout, 
                "Advanced Settings", 
                menuAction: ShowAdvancedMenu, 
                menuIcon: SoundEventDataPropertyDrawer.PresetStyle());
            
            void ShowAdvancedMenu(Rect position)
            {
                var menu = new GenericMenu();

                for (int i = 0; i < AudiophileProjectSettings.AdvancedSettings.Length; i++)
                {
                    string name = AudiophileProjectSettings.AdvancedSettings[i].Name;
                    menu.AddItem(new GUIContent($"{i} : {name}"), false, OnAdvancedResetClicked, i);
                }

                menu.DropDown(position);
            }

            void OnAdvancedResetClicked(object index)
            {
                var setting = AudiophileProjectSettings.AdvancedSettings[(int)index];

                var so = new SerializedObject(AudiophileProjectSettings.Instance);
                var sourceProp = so.FindProperty("advancedSettings").GetArrayElementAtIndex((int)index).FindPropertyRelative("data");

                property.serializedObject.Update();
                PrefabEvolution.PEPropertyHelper.CopyPropertyValue(sourceProp, property);
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndFoldoutHeaderGroup();
            position.y += ExtraEditorGUIUtility.SingleLineHeight();
            if (AdvancedSettingsSectionFoldout)
            {
                var bypassEffectsProp = property.FindPropertyRelative("bypassEffects");
                EditorGUI.PropertyField(position, bypassEffectsProp);
                position.y += ExtraEditorGUIUtility.SingleLineHeight();

                var bypassListenerEffectsProp = property.FindPropertyRelative("bypassListenerEffects");
                EditorGUI.PropertyField(position, bypassListenerEffectsProp);
                position.y += ExtraEditorGUIUtility.SingleLineHeight();

                var bypassReverbZonesProp = property.FindPropertyRelative("bypassReverbZones");
                EditorGUI.PropertyField(position, bypassReverbZonesProp);
                position.y += ExtraEditorGUIUtility.SingleLineHeight();

                if (!bypassReverbZonesProp.boolValue)
                {
                    var reverbZoneMixProp = property.FindPropertyRelative("reverbZoneMix");
                    EditorGUI.PropertyField(position, reverbZoneMixProp);
                    position.y += ExtraEditorGUIUtility.SingleLineHeight();
                }
            }
        }
    }
}