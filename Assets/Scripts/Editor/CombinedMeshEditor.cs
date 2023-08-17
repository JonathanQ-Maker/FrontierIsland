using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MeshCombiner))]
public class ItemIconsEditor : Editor
{
    MeshCombiner meshCombiner;
    private void OnEnable()
    {
        meshCombiner = (MeshCombiner)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Combine Mesh"))
        {
            meshCombiner.CombineMesh();
            EditorUtility.SetDirty(meshCombiner.gameObject);
        }

        if (GUILayout.Button("Save Mesh"))
        {
            string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", "CombinedMesh", "asset");
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path);
            meshCombiner.SaveMesh(path);
        }
    }
}
