﻿using Bewildered.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PixelDust.Audiophile
{
    [CustomPropertyDrawer(typeof(SoundEvent))]
    public class SoundEventPropertyDrawer : PropertyDrawer
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
            //headerRect.width -= PLAY_BUTTON_WIDTH * 2 + PLAY_BUTTON_GAP;
            headerRect.width = EditorGUIUtility.labelWidth;

            if (property.IsArrayElement())
            {
                var soundEvent = property.GetValue<SoundEvent>();
                if(soundEvent.Data.StandardSettings.MinVolume == 0 && soundEvent.Data.StandardSettings.MaxVolume == 0)
                {
                    soundEvent.Data.Reset();
                }
            }

            GUIContent newLabel = label;

            //TODO: Add back
            //Texture2D tex = Resources.Load("se") as Texture2D;
            //newLabel.image = tex;
            
            var preset = property.FindPropertyRelative("preset");
            SerializedProperty dataProp = property.FindPropertyRelative("data");
            SerializedProperty selDataProp = dataProp;

            if (preset.objectReferenceValue != null)
            {
                var presetSO = new SerializedObject(preset.objectReferenceValue);
                selDataProp = presetSO.FindProperty("data");
            }

            Rect playPos = position;
            playPos.height = EditorGUIUtility.singleLineHeight;

            DrawPlayStop(playPos, selDataProp);

            if (preset.objectReferenceValue == null)
            {
                property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(headerRect, property.isExpanded, newLabel);
                EditorGUI.EndFoldoutHeaderGroup();

                Rect presetRect = position;
                presetRect.x += EditorGUIUtility.labelWidth;
                presetRect.width -= (PLAY_BUTTON_WIDTH * 2 + PLAY_BUTTON_GAP + EditorGUIUtility.labelWidth);
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
                presetRect.width -= PLAY_BUTTON_WIDTH * 2 + PLAY_BUTTON_GAP;
                EditorGUI.PropertyField(presetRect, preset, label, true);
            }

            EditorGUI.EndProperty();
        }

        public const float PLAY_BUTTON_WIDTH = 20;
        public const float PLAY_BUTTON_GAP = 4;
        public static void DrawPlayStop(Rect position, SerializedProperty property, Action<SerializedProperty> onPlay, Action<SerializedProperty> onStop)
        {
            Rect playPos = position;
            playPos.x = (position.x + position.width) - PLAY_BUTTON_WIDTH - PLAY_BUTTON_WIDTH;
            playPos.width = PLAY_BUTTON_WIDTH;
            playPos.height = PLAY_BUTTON_WIDTH;
            if (GUI.Button(playPos, EditorGUIUtility.IconContent("d_PlayButton@2x"), EditorStyles.miniButtonMid))
            {
                onPlay?.Invoke(property);
            }
            Rect stopPos = position;
            stopPos.x = (position.x + position.width) - PLAY_BUTTON_WIDTH;
            stopPos.width = PLAY_BUTTON_WIDTH;
            stopPos.height = PLAY_BUTTON_WIDTH;
            if (GUI.Button(stopPos, EditorGUIUtility.IconContent("d_PauseButton@2x"), EditorStyles.miniButtonMid))
            {
                onStop?.Invoke(property);
            }
        }

        /// <summary>
        /// Expects the serialized property to be a SoundEvent
        /// </summary>
        /// <param name="position"></param>
        /// <param name="soundEventDataProp"></param>
        public static void DrawPlayStop(Rect position, SerializedProperty soundEventDataProp)
        {
            DrawPlayStop(position, soundEventDataProp, OnPlay, OnStop);
            void OnPlay(SerializedProperty obj)
            {
                PropertyDrawerUtility.GetTargetObjectOfProperty<SoundEventData>(obj, out SoundEventData soundEventData);
                SoundManager.ProcessSound(soundEventData);
            }

            void OnStop(SerializedProperty obj)
            {
                PropertyDrawerUtility.GetTargetObjectOfProperty<SoundEventData>(obj, out SoundEventData soundEventData);
                SoundManager.StopSound(soundEventData.SoundId);
            }
        }
    }
}