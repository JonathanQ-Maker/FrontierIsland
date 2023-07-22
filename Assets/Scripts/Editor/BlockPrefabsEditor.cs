using UnityEditor;
using UnityEngine;
using System;
using UnityEditorInternal;

namespace FrontierIsland
{
    [CustomEditor(typeof(BlockPrefabs))]
    public class BlockPrefabsEditor : Editor
    {
        // see: https://xinyustudio.wordpress.com/2015/07/21/unity3d-using-reorderablelist-in-custom-editor/
        ReorderableList list;
        bool prefabCollapsed;

        private void OnEnable()
        {
            list = new ReorderableList(serializedObject, serializedObject.FindProperty("prefabs"), true, true, true, true);
        }

        public override void OnInspectorGUI()
        {
            prefabCollapsed = EditorGUILayout.Foldout(prefabCollapsed, "Prefabs");

            if (prefabCollapsed)
            {
                list.DoLayoutList();
                list.onCanAddCallback = (ReorderableList list) =>
                {
                    return list.count < Enum.GetValues(typeof(BlockType)).Length;
                };
                list.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Prefabs");
                };
                list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty prefab = list.serializedProperty.GetArrayElementAtIndex(index);

                    Rect labelRect = new Rect(rect.x, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight);
                    Rect prefabRect = new Rect(rect.x + labelRect.width, rect.y,
                        rect.width * 0.6f, EditorGUIUtility.singleLineHeight);

                    BlockType type = (BlockType)index;

                    EditorGUI.LabelField(labelRect, type.ToString());
                    prefab.objectReferenceValue = EditorGUI.ObjectField(prefabRect,
                        prefab.objectReferenceValue, typeof(Block), false);
                };
            }
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}