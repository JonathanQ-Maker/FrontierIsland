using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    public Mesh combinedMesh;
    public void CombineMesh()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(true);
        if (meshFilters.Length <= 0) return;
        
        Vector3 originalPos = transform.position;
        Quaternion originalRotation = transform.rotation;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        CombineInstance[] combine = new CombineInstance[meshFilters.Length-1];

        int index = 0;
        for (int i = 0; i < meshFilters.Length; ++i)
        {
            GameObject gameObject = meshFilters[i].gameObject;
            if (!ReferenceEquals(gameObject, this.gameObject))
            {
                combine[index].mesh = meshFilters[i].sharedMesh;
                combine[index].transform = meshFilters[i].transform.localToWorldMatrix;
                gameObject.SetActive(false);
                ++index;
            }
        }
        transform.position = originalPos;
        transform.rotation = originalRotation;

        
        combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = combinedMesh;
    }
}
