using System.Collections.Generic;
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
        private RecipeGraphUI helpPanel;
        [SerializeField]
        private Canvas mainCanvas, worldCanvas;

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

        public Canvas WorldCanvas
        {
            get { return worldCanvas; }
        }

        private Settler settler;

        public readonly HashSet<Settler> settlers = new HashSet<Settler>(3);

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

        // TODO: cube color based on correct/not correct use
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

            if (Input.GetKeyDown(KeyCode.D))
            {
                System.GC.Collect();
                Debug.Log("Force Garbage collector");
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log($"DebugTracker instance counts: {DebugTracker.Count}");
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                for (int i = 0; i < settler.Inventory.Capacity; ++i)
                {
                    ItemStack itemStack = settler.Inventory[i];
                    if (itemStack != null)
                    Debug.Log($"[{i}]: {itemStack.name} {itemStack.count}");
                }
            }
        }

        private void ItemSlotClick(int index, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                SelectSlot(index);
        }

        private void SelectSlot(int index)
        { 
            hotBarWindow.SelectionIndex = index;
            settler.HeldItemIndex = hotBarWindow.SelectionIndex;
        }

        private void HandleHotBarInput()
        {
            if (settler != null)
            for (int i = (int)KeyCode.Alpha1; i <= (int)KeyCode.Alpha9; ++i)
            {
                if (Input.GetKeyDown((KeyCode)i))
                {
                    int selectedSlot = i - ((int)KeyCode.Alpha0) - 1; // minus one to align with keyboard
                    SelectSlot(selectedSlot);
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
            if (Input.GetKeyDown(KeyCode.Space) && settler != null)
            {
                CameraManager.Instance.StartFocus(settler.transform.position);
            }
        }

        private void HandleHelpPanel()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                helpPanel.Active = !helpPanel.Active;
            }
        }

        private void OnSelect(ISelectable selectable, Vector3Int pos, Vector3 normal)
        {
            if (selectable is Settler)
            {
                if (!ReferenceEquals(selectable, settler))
                {
                    settler = (Settler)selectable;
                    hotBarWindow.Active = true;
                    hotBarWindow.Inventory = settler.Inventory;
                    hotBarWindow.SelectionIndex = settler.HeldItemIndex;
                    hotBarWindow.itemSlotClick = ItemSlotClick;
                    SetAllHide(settler);
                }
                else
                {
                    settler.OpenUI();
                }
                CameraManager.Instance.StartFocus(settler.transform.position);
                return;
            }
            
            // sanity check
            if (selectable is Block)
                Assert.IsTrue(ReferenceEquals(Terrain.Instance.GetBlock(((Block)selectable).Position), selectable));
            

            if (settler != null)
            {
                settler.CloseUI();
                settler.CloseView();

                if (selectable is ContainerBlock craftingBlock && !Input.GetKey(KeyCode.LeftControl)) 
                {
                    settler.StartViewBlock(craftingBlock.Position);
                    return;
                }

                if (selectable is ItemHandler handler)
                {
                    settler.StartCollectItem(handler);
                    return;
                }

                if (settler.HeldItem != null && settler.HeldItem.CanPlaceBlock)
                {
                    BlockFace face = GetSelectBlockFace(normal);
                    switch (face)
                    {
                        case BlockFace.North:
                            settler.StartUsePlaceBlock(pos + Vector3Int.forward, face);
                            break;
                        case BlockFace.East:
                            settler.StartUsePlaceBlock(pos + Vector3Int.right, face);
                            break;
                        case BlockFace.South:
                            settler.StartUsePlaceBlock(pos + Vector3Int.back, face);
                            break;
                        case BlockFace.West:
                            settler.StartUsePlaceBlock(pos + Vector3Int.left, face);
                            break;
                        case BlockFace.Top:
                            settler.StartUsePlaceBlock(pos + Vector3Int.up, face);
                            break;
                    }
                    return;
                }

                if (selectable is Block block)
                {
                    settler.StartHarvestBlock(block.Position);
                    return;
                }


                if (selectable is Chunk)
                {
                    settler.StartMoveTo(pos);
                }
            }
        }

        private void SetAllHide(Settler except)
        {
            foreach (Settler settler in settlers)
            {
                settler.ShowView = ReferenceEquals(except, settler);
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