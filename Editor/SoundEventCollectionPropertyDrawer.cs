using System;
using UnityEditor;
using UnityEngine;

namespace PixelDust.Audiophile
{
    [CustomPropertyDrawer(typeof(SoundEventCollection))]
    public class SoundEventCollectionPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;

            //Determine where we are receiving the data from
            //Get the preset asset
            var preset = property.FindPropertyRelative("preset");

            //Get the data since we need to delete its managed reference if using a preset
            //And create its managed reference if not using a preset
            SerializedProperty data = property.FindPropertyRelative("data");
            height += ExtraEditorGUIUtility.SingleLineHeight();

            if (property.isExpanded)
            {
                //Draw Preset Field
                if (preset.objectReferenceValue == null)
                {
                    height += EditorGUI.GetPropertyHeight(data);
                    height += EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position.height = EditorGUIUtility.singleLineHeight;

            Rect headerRect = EditorGUI.IndentedRect(position);
            //headerRect.width -= SoundEventPropertyDrawer.PLAY_BUTTON_WIDTH * 2 + SoundEventPropertyDrawer.PLAY_BUTTON_GAP;
            headerRect.width = EditorGUIUtility.labelWidth;

            //Get the data since we need to delete its managed reference if using a preset
            //And create its managed reference if not using a preset
            SerializedProperty dataProp = property.FindPropertyRelative("data");
            SerializedProperty selDataProp = dataProp;
            //Determine where we are receiving the data from
            var preset = property.FindPropertyRelative("preset");

            GUIContent newLabel = label;

            //TODO: Add back
            //Texture2D tex = Resources.Load("se-col") as Texture2D;
            //newLabel.image = tex;

            if (preset.objectReferenceValue != null)
            {
                var presetSO = new SerializedObject(preset.objectReferenceValue);
                selDataProp = presetSO.FindProperty("data");
            }

            Rect playPos = position;
            playPos.height = EditorGUIUtility.singleLineHeight;

            SoundEventPropertyDrawer.DrawPlayStop(playPos, selDataProp, OnPlay, OnStop);

            if (preset.objectReferenceValue == null)
            {
                property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(headerRect, property.isExpanded, newLabel);
                EditorGUI.EndFoldoutHeaderGroup();

                Rect presetRect = position;
                presetRect.x += EditorGUIUtility.labelWidth;
                presetRect.width -= (SoundEventPropertyDrawer.PLAY_BUTTON_WIDTH * 2 + SoundEventPropertyDrawer.PLAY_BUTTON_GAP + EditorGUIUtility.labelWidth);
                EditorGUI.PropertyField(presetRect, preset, GUIContent.none, true);
                position.y += ExtraEditorGUIUtility.SingleLineHeight();

                //position.y += ExtraEditorGUIUtility.SingleLineHeight();

                if (property.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    {
                        EditorGUI.PropertyField(position, dataProp, true);
                    }
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                Rect presetRect = position;
                presetRect.width -= SoundEventPropertyDrawer.PLAY_BUTTON_WIDTH * 2 + SoundEventPropertyDrawer.PLAY_BUTTON_GAP;
                EditorGUI.PropertyField(presetRect, preset, newLabel, true);
            }

            EditorGUI.EndProperty();
        }

        public static void OnPlay(SerializedProperty dataProp)
        {
            SoundEventCollectionData soundEventCollectionData = null;
            PropertyDrawerUtility.GetTargetObjectOfProperty<SoundEventCollectionData>(dataProp, out soundEventCollectionData);
            soundEventCollectionData?.PlayAt(Vector3.zero);
        }

        public static void OnStop(SerializedProperty dataProp)
        {
            SoundEventCollectionData soundEventCollectionData = null;
            PropertyDrawerUtility.GetTargetObjectOfProperty<SoundEventCollectionData>(dataProp, out soundEventCollectionData);
            SoundManager.StopSound(soundEventCollectionData.SoundId);
        }
    }
}