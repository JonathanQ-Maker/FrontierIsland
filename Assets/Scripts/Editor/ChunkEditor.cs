using UnityEditor;
using UnityEngine;
using System;

namespace FrontierIsland
{
    [CustomEditor(typeof(Chunk))]
    public class ChunkEditor : Editor
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
