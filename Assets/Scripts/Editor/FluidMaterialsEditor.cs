using UnityEditor;
using UnityEngine;
using System;
using UnityEditorInternal;

namespace FrontierIsland
{
    [CustomEditor(typeof(FluidMaterials))]
    public class FluidMaterialsEditor : Editor
    {
        // see: https://xinyustudio.wordpress.com/2015/07/21/unity3d-using-reorderablelist-in-custom-editor/
        ReorderableList sceneMatList, uiMatList;

        private void OnEnable()
        {
            sceneMatList = new ReorderableList(serializedObject, serializedObject.FindProperty("sceneMaterials"), true, true, true, true);
            sceneMatList.onCanAddCallback = (ReorderableList list) =>
            {
                return list.count < Enum.GetValues(typeof(FluidType)).Length;
            };
            sceneMatList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Scene Materials");
            };
            sceneMatList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty prefab = sceneMatList.serializedProperty.GetArrayElementAtIndex(index);

                Rect labelRect = new Rect(rect.x, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight);
                Rect prefabRect = new Rect(rect.x + labelRect.width, rect.y,
                    rect.width * 0.6f, EditorGUIUtility.singleLineHeight);

                FluidType type = (FluidType)index;

                EditorGUI.LabelField(labelRect, type.ToString());
                prefab.objectReferenceValue = EditorGUI.ObjectField(prefabRect,
                    prefab.objectReferenceValue, typeof(Material), false);
            };



            uiMatList = new ReorderableList(serializedObject, serializedObject.FindProperty("uiMaterials"), true, true, true, true);
            uiMatList.onCanAddCallback = (ReorderableList list) =>
            {
                return list.count < Enum.GetValues(typeof(FluidType)).Length;
            };
            uiMatList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "UI Materials");
            };
            uiMatList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty prefab = uiMatList.serializedProperty.GetArrayElementAtIndex(index);

                Rect labelRect = new Rect(rect.x, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight);
                Rect prefabRect = new Rect(rect.x + labelRect.width, rect.y,
                    rect.width * 0.6f, EditorGUIUtility.singleLineHeight);

                FluidType type = (FluidType)index;

                EditorGUI.LabelField(labelRect, type.ToString());
                prefab.objectReferenceValue = EditorGUI.ObjectField(prefabRect,
                    prefab.objectReferenceValue, typeof(Material), false);
            };
        }

        public override void OnInspectorGUI()
        {
            sceneMatList.DoLayoutList();
            uiMatList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}