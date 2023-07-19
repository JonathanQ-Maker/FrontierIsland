using UnityEngine;
namespace FrontierIsland
{
    public class GameController : MonoBehaviour
    {
        public Settler entity;

        private void Awake()
        {
            // enables debug if development build, otherwise disable log
            Debug.unityLogger.logEnabled = Debug.isDebugBuild;
        }


        public Transform cube;

        private void Update()
        {
            HandleSelect();
        }


        public enum BlockFace
        {
            Top,
            North,
            East,
            South,
            West
        }

        private void HandleSelect()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                float x = hit.point.x, y = hit.point.y, z = hit.point.z;
                Vector3Int pos = new Vector3Int();

                if (x - ray.origin.x > 0)
                    pos.x = Mathf.FloorToInt(x + 0.5f);
                else
                    pos.x = Mathf.CeilToInt(x - 0.5f);

                if (z - ray.origin.z > 0)
                    pos.z = Mathf.FloorToInt(z + 0.5f);
                else
                    pos.z = Mathf.CeilToInt(z - 0.5f);

                pos.y = Mathf.FloorToInt(Mathf.Clamp(y - 0.01f, -1, 3));

                if (hit.collider.gameObject.TryGetComponent(out ISelectable selectable))
                {
                    if (selectable is Block || selectable is Chunk)
                    {
                        if (!cube.gameObject.activeSelf)
                            cube.gameObject.SetActive(true);
                        cube.position = pos;
                    }
                    else if (cube.gameObject.activeSelf)
                    {
                        cube.gameObject.SetActive(false);
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        selectable.OnSelect();
                        OnSelect(selectable, pos);
                    }

                    if (Input.GetKeyDown(KeyCode.D))
                    {
                        if (selectable is MultiBlock)
                        {
                            Terrain.Instance.DestroyMultiBlock((MultiBlock)selectable);
                        }
                    }
                }
                else if (cube.gameObject.activeSelf)
                {
                    cube.gameObject.SetActive(false);
                }
            }
            else
            {
                if (cube.gameObject.activeSelf)
                    cube.gameObject.SetActive(false);
            }
        }

        private void OnSelect(ISelectable selectable, Vector3Int pos)
        {
            if (selectable is Settler)
            {
                entity = (Settler)selectable;
            }

            if (entity != null)
            {
                if (selectable is Chunk)
                {
                    entity.StartMoveTo(pos);
                }

                if (selectable is Block)
                {
                    entity.StartHarvestBlock((Block)selectable);
                }
            }

            //if (selectable is Chunk)
            //{
            //    //Terrain.Instance.SetTile(TileType.Air, pos.x, pos.z, true);
            //    Block block = Terrain.Instance.PlaceBlock(BlockType.Tree, pos + Vector3Int.up);
            //    Debug.Log(block != null);
            //}
        }

        private BlockFace GetSelectBlockFace(Vector3 normal)
        {
            float absX = Mathf.Abs(normal.x),
                absY = Mathf.Abs(normal.y),
                absZ = Mathf.Abs(normal.z);
            if (absY > absX && absY > absZ)
            {
                return BlockFace.Top;
            }

            if (absX > absZ)
            {
                return normal.x > 0 ? BlockFace.East : BlockFace.West;
            }
            return normal.z > 0 ? BlockFace.North : BlockFace.South;
        }
    }
}