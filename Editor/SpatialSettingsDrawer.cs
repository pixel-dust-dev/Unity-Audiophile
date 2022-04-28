using UnityEditor;
using UnityEngine;

namespace PixelDust.Audiophile
{
    [CustomPropertyDrawer(typeof(SpatialSettings))]
    public class SpatialSettingsDrawer : PropertyDrawer
    {
        const string SPATIAL_SETTINGS_SECTION_FOLDOUT_KEY = "PixelDust.Audiophile.SoundEventDataPropertyDrawer.SpatialSettingsSectionFoldout";
        static bool SpatialSettingsSectionFoldout
        {
            get
            {
                return EditorPrefs.GetBool(SPATIAL_SETTINGS_SECTION_FOLDOUT_KEY);
            }
            set
            {
                EditorPrefs.SetBool(SPATIAL_SETTINGS_SECTION_FOLDOUT_KEY, value);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;

            height += ExtraEditorGUIUtility.SingleLineHeight();
            if (SpatialSettingsSectionFoldout)
            {
                var is3DProp = property.FindPropertyRelative("is3D");
                if (is3DProp.boolValue)
                {
                    height += ExtraEditorGUIUtility.SingleLineHeight() * 5;
                }
                else
                {
                    height += ExtraEditorGUIUtility.SingleLineHeight();
                }
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float is3DWidth = 60;

            position.height = EditorGUIUtility.singleLineHeight;

            Rect spatialSettingsRect = EditorGUI.IndentedRect(position);
            spatialSettingsRect.width -= is3DWidth;
            SpatialSettingsSectionFoldout = EditorGUI.BeginFoldoutHeaderGroup(
                spatialSettingsRect, 
                SpatialSettingsSectionFoldout, 
                "Spatial Settings", 
                menuAction: ShowSpatialMenu,
                menuIcon: SoundEventDataPropertyDrawer.PresetStyle());

            void ShowSpatialMenu(Rect pos)
            {
                var menu = new GenericMenu();

                for (int i = 0; i < AudiophileProjectSettings.SpatialSettings.Length; i++)
                {
                    string name = AudiophileProjectSettings.SpatialSettings[i].Name;
                    menu.AddItem(new GUIContent($"{i} : {name}"), false, OnSpatialResetClicked, i);
                }

                menu.DropDown(pos);
            }

            void OnSpatialResetClicked(object index)
            {
                var setting = AudiophileProjectSettings.SpatialSettings[(int)index];

                var so = new SerializedObject(AudiophileProjectSettings.Instance);
                var sourceProp = so.FindProperty("spatialSettings").GetArrayElementAtIndex((int)index).FindPropertyRelative("data");

                property.serializedObject.Update();
                PrefabEvolution.PEPropertyHelper.CopyPropertyValue(sourceProp, property);
                property.serializedObject.ApplyModifiedProperties();
            }

            var is3DProp = property.FindPropertyRelative("is3D");
            Rect toggle3DRect = spatialSettingsRect;
            toggle3DRect.width = is3DWidth;
            toggle3DRect.x += spatialSettingsRect.width;

            var spatialTypeStrings = new string[] { "3D", "2D" };

            if(is3DProp.hasMultipleDifferentValues)
            {
                EditorGUI.showMixedValue = true;
            }
            int spatialType = GUI.Toolbar(toggle3DRect, is3DProp.boolValue ? 0 : 1, spatialTypeStrings, EditorStyles.miniButtonMid);
            is3DProp.boolValue = spatialType == 0 ? true : false;
            EditorGUI.showMixedValue = false;

            EditorGUI.EndFoldoutHeaderGroup();
            position.y += ExtraEditorGUIUtility.SingleLineHeight();
            if (SpatialSettingsSectionFoldout)
            {
                if (is3DProp.boolValue)
                {
                    var dopplerLevelProp = property.FindPropertyRelative("dopplerLevel");
                    EditorGUI.PropertyField(position, dopplerLevelProp);
                    position.y += ExtraEditorGUIUtility.SingleLineHeight();

                    var spreadProp = property.FindPropertyRelative("spread");
                    EditorGUI.PropertyField(position, spreadProp);
                    position.y += ExtraEditorGUIUtility.SingleLineHeight();

                    var volumeRolloffProp = property.FindPropertyRelative("volumeRolloff");
                    EditorGUI.PropertyField(position, volumeRolloffProp);
                    position.y += ExtraEditorGUIUtility.SingleLineHeight();

                    var minDistanceProp = property.FindPropertyRelative("minDistance");
                    EditorGUI.PropertyField(position, minDistanceProp);
                    position.y += ExtraEditorGUIUtility.SingleLineHeight();

                    var maxDistanceProp = property.FindPropertyRelative("maxDistance");
                    EditorGUI.PropertyField(position, maxDistanceProp);
                    position.y += ExtraEditorGUIUtility.SingleLineHeight();
                }
                else
                {
                    var stereoPanMinProp = property.FindPropertyRelative("stereoPanMin");
                    var stereoPanMaxProp = property.FindPropertyRelative("stereoPanMax");
                    SoundEventDataPropertyDrawer.DrawRangeSlider(ref position, new GUIContent("Stereo Pan"), stereoPanMinProp, stereoPanMaxProp, -1, 1, AudiophileProjectSettings.Units.Linear);
                }
            }
        }
    }
}