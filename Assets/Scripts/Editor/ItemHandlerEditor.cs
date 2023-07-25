using UnityEditor;
using UnityEngine;
using System;

namespace FrontierIsland
{
    [CustomEditor(typeof(ItemHandler))]
    public class ItemHandlerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Update Mesh"))
            {
                ((Chunk)target).UpdateChunkMesh();
            }
        }
    }
}
