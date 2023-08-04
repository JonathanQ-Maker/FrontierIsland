using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace FrontierIsland
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private BlockPrefabs blockPrefabs;
        [SerializeField]
        private ItemHandlerPrefabs itemHandlerPrefabs;
        [SerializeField]
        private ItemIcons itemIcons;
        [SerializeField]
        private HotBarUI hotBarWindow;
        [SerializeField]
        private ToolTipUI toolTipWindow;
        [SerializeField]
        private RectTransform helpPanel;
        [SerializeField]
        private Canvas mainCanvas;

        public ItemHandlerPrefabs ItemHandlerPrefabs
        {
            get { return itemHandlerPrefabs;  }
        }

        public BlockPrefabs BlockPrefabs 
        { 
            get { return blockPrefabs; } 
        }

        public ItemIcons ItemIcons
        {
            get { return itemIcons; }
        }
        
        public ToolTipUI ToolTipWindow
        {
            get { return toolTipWindow; }
        }

        public Canvas MainCanvas
        {
            get { return mainCanvas; }
        }

        private Settler settler;

        public static GameController Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("More than one instance of terrain exists");
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            // enables debug if development build, otherwise disable log
            Debug.unityLogger.logEnabled = Debug.isDebugBuild;

            ItemAtlas.SetUp();
            RecipeCollections.SetUp();
        }


        public Transform cube;


        private void Start()
        {
            cube.rotation = Quaternion.identity;
        }
        private void Update()
        {
            HandleSelect();
            HandleHotBarInput();
            HandleFocus();
            HandleHelpPanel();


            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log($"ItemStack instance counts: {ItemStack.ItemCount}");
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                System.GC.Collect();
                Debug.Log("Force Garbage collector");
            }
        }


        public enum BlockFace
        {
            Top,
            North,
            East,
            South,
            West
        }

        private void HandleHotBarInput()
        {
            if (settler != null)
            for (int i = (int)KeyCode.Alpha1; i <= (int)KeyCode.Alpha9; ++i)
            {
                if (Input.GetKeyDown((KeyCode)i))
                {
                    int selectedSlot = i - ((int)KeyCode.Alpha0) - 1; // minus one to align with keyboard

                    hotBarWindow.SelectionIndex = selectedSlot;
                    settler.HeldItemIndex = hotBarWindow.SelectionIndex;
                }
            }
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
                    if (selectable is Block || selectable is Chunk || selectable is ItemHandler)
                    {
                        if (!cube.gameObject.activeSelf)
                            cube.gameObject.SetActive(true);
                        cube.position = pos;
                    }
                    else if (cube.gameObject.activeSelf)
                    {
                        cube.gameObject.SetActive(false);
                    }

                    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        selectable.OnSelect();
                        OnSelect(selectable, pos, hit.normal);
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

        private void HandleFocus()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CameraManager.Instance.StartFocus(settler.transform.position);
            }
        }

        private void HandleHelpPanel()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                helpPanel.gameObject.SetActive(!helpPanel.gameObject.activeSelf);
            }
        }

        private void OnSelect(ISelectable selectable, Vector3Int pos, Vector3 normal)
        {
            if (selectable is Settler)
            {
                settler = (Settler)selectable;
                hotBarWindow.Active = true;
                hotBarWindow.Inventory = settler.Inventory;
                hotBarWindow.SelectionIndex = settler.HeldItemIndex;
                CameraManager.Instance.StartFocus(settler.transform.position);
            }

            if (settler != null)
            {
                if (settler.HeldItem is BlockItem)
                {
                    if (selectable is Chunk)
                    {
                        settler.StartPlaceBlock(pos + Vector3Int.up);
                        return;
                    }
                    else if (selectable is Block)
                    {
                        BlockFace face = GetSelectBlockFace(normal);

                        switch (face)
                        {
                            case BlockFace.North:
                                settler.StartPlaceBlock(pos + Vector3Int.forward);
                                break;
                            case BlockFace.East:
                                settler.StartPlaceBlock(pos + Vector3Int.right);
                                break;
                            case BlockFace.South:
                                settler.StartPlaceBlock(pos + Vector3Int.back);
                                break;
                            case BlockFace.West:
                                settler.StartPlaceBlock(pos + Vector3Int.left);
                                break;
                            case BlockFace.Top:
                                settler.StartPlaceBlock(pos + Vector3Int.up);
                                break;
                        }
                        return;
                    }
                }


                if (selectable is Chunk)
                {
                    settler.StartMoveTo(pos);
                }

                if (selectable is Block block)
                {
                    Assert.IsTrue(ReferenceEquals(Terrain.Instance.GetBlock(block.Position), selectable));

                    if (selectable is CraftingBlock)
                    {
                        ((CraftingBlock)selectable).ShowUI = !((CraftingBlock)selectable).ShowUI;
                    }
                    else
                    {
                        settler.StartHarvestBlock(block);
                    }
                }

                if (selectable is ItemHandler handler)
                {
                    settler.StartCollectItem(handler);
                }
            }
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