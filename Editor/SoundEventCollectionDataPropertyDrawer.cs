using UnityEditor;
using UnityEngine;

namespace PixelDust.Audiophile
{
    [CustomPropertyDrawer(typeof(SoundEventCollectionData))]
    public class SoundEventCollectionDataPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;
            height += ExtraEditorGUIUtility.SingleLineHeight() * 1;
            var soundEventCollectionProp = property.FindPropertyRelative("soundEventCollection");
            if (soundEventCollectionProp != null)
            {
                height += EditorGUI.GetPropertyHeight(soundEventCollectionProp, true);
            }
            if (property.serializedObject.targetObject is SoundEventCollectionPreset)
            {
                height += ExtraEditorGUIUtility.SingleLineHeight();
            }
            height += EditorGUIUtility.standardVerticalSpacing;
            return height;
        }

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            SoundEventCollectionData soundEventCollectionData;
            if (!PropertyDrawerUtility.GetTargetObjectOfProperty<SoundEventCollectionData>(property, out soundEventCollectionData))
            {
                Debug.LogError("There was a problem.");
                return;
            }

            Draw(pos, property, label, false);
        }

        public static void Draw(Rect position, SerializedProperty property, GUIContent label, bool embedded)
        {
            EditorGUI.BeginProperty(position, label, property);

            SoundEventCollectionData soundEventCollectionData = null;
            PropertyDrawerUtility.GetTargetObjectOfProperty<SoundEventCollectionData>(property, out soundEventCollectionData);
            DrawData(ref position, property, soundEventCollectionData, embedded);

            EditorGUI.EndProperty();
        }

        private static void DrawData(ref Rect position, SerializedProperty data, SoundEventCollectionData soundEventCollectionData, bool embedded)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            if (data.serializedObject.targetObject is SoundEventCollectionPreset)
            {
                Rect playStopRect = position;
                SoundEventPropertyDrawer.DrawPlayStop(playStopRect, data, SoundEventCollectionPropertyDrawer.OnPlay, SoundEventCollectionPropertyDrawer.OnStop);
                position.y += ExtraEditorGUIUtility.SingleLineHeight();
            }

            EditorGUI.BeginDisabledGroup(embedded);
            {
                var soundEventsCollectionProp = data.FindPropertyRelative("soundEventCollection");
                {
                    Rect clipsPos = position;
                    clipsPos.x += 14;
                    clipsPos.width -= 14;
                    EditorGUI.PropertyField(clipsPos, soundEventsCollectionProp, new GUIContent("Sound Events"), true);
                    var propHeight = EditorGUI.GetPropertyHeight(soundEventsCollectionProp, true);
                    position.y += propHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                var soundIdProp = data.FindPropertyRelative("soundId");
                {
                    if (string.IsNullOrEmpty(soundIdProp.stringValue))
                    {
                        soundIdProp.stringValue = GUID.Generate().ToString();
                    }
                    EditorGUI.PropertyField(position, soundIdProp, true);
                    var propHeight = EditorGUI.GetPropertyHeight(soundIdProp, true);
                    position.y += propHeight + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}