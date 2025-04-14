using UnityEditor;
using UnityEngine;
using System;
using UnityEditorInternal;

namespace FrontierIsland
{
    [CustomEditor(typeof(ItemIcons))]
    public class ItemIconsEditor : Editor
    {
        // see: https://xinyustudio.wordpress.com/2015/07/21/unity3d-using-reorderablelist-in-custom-editor/
        ReorderableList icons;

        private void OnEnable()
        {
            icons = new ReorderableList(serializedObject, serializedObject.FindProperty("icons"), true, true, true, true);
            icons.onCanAddCallback = (ReorderableList list) =>
            {
                return icons.count < Enum.GetValues(typeof(ItemType)).Length;
            };
            icons.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Icons");
            };
            icons.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty icon = icons.serializedProperty.GetArrayElementAtIndex(index);

                Rect labelRect = new Rect(rect.x, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight);
                Rect iconRect = new Rect(rect.x + labelRect.width, rect.y,
                    rect.width * 0.3f, EditorGUIUtility.singleLineHeight);

                ItemType type = (ItemType)index;

                EditorGUI.LabelField(labelRect, type.ToString());
                icon.objectReferenceValue = EditorGUI.ObjectField(iconRect,
                    icon.objectReferenceValue, typeof(Sprite), false);
            };
        }

        public override void OnInspectorGUI()
        {
            icons.DoLayoutList();


            serializedObject.ApplyModifiedProperties();
        }
    }
}