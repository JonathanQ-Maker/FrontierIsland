using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class RecipeGraphUI : WindowedDragRect
    {
        [SerializeField]
        private RecipeNodeUI graphNodePrefab;

        [SerializeField]
        private RecipeMenuItem recipeMenuItemPrefab;

        [SerializeField]
        private Image directedLinePrefab;

        [SerializeField]
        private RectTransform edgeParent;

        [SerializeField]
        private RectTransform menuContent;

        [SerializeField]
        public RectTransform selectOverlay;

        private Dictionary<ItemType, RecipeMenuItem> menuItems 
            = new Dictionary<ItemType, RecipeMenuItem>(RecipeGraph.Keys.Count);

        public ItemType CurrentItem { get; private set; }

        public const int CAMERA_CONTROL_MASK = 1;
        public bool Active 
        {
            get { return gameObject.activeSelf; }
            set 
            {
                gameObject.SetActive(value);
                if (value)
                {
                    CameraManager.Instance.dragFlag |= CAMERA_CONTROL_MASK;
                    CameraManager.Instance.scrollFlag |= CAMERA_CONTROL_MASK;
                }
                else
                {
                    CameraManager.Instance.dragFlag &= ~CAMERA_CONTROL_MASK;
                    CameraManager.Instance.scrollFlag &= ~CAMERA_CONTROL_MASK;
                }
            }
        }

        private void Start()
        {
            // menu must be render first because RenderGraph()
            // depends on a full menuItems dictionary
            RenderMenu();
            RenderGraph(ItemType.StoneAxe);
        }

        #region graph
        public void RenderGraph(ItemType itemType)
        {
            if (itemType == CurrentItem || !RecipeGraph.TryGetNode(itemType, out RecipeGraph.RecipeGraphNode graphNode)) return;
            ClearGraph();
            CurrentItem = itemType;

            selectOverlay.gameObject.SetActive(true);
            selectOverlay.SetParent(menuItems[itemType].transform, false);

            RecipeNodeUI centerNode = InstantiateNode(itemType, Vector2.zero);

            float r = 150;

            int i = 0;
            foreach (ItemType item in graphNode.requiredBy)
            {
                int segments = graphNode.requiredBy.Count + 1;
                float angle = (i + 1) * Mathf.PI / segments;
                RecipeNodeUI node = InstantiateNode(item, new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r));
                ++i;
                CreateEdge(centerNode.rectTransform.anchoredPosition, node.rectTransform.anchoredPosition, edgeParent);
            }

            i = 0;
            foreach (Ingredient ingredient in graphNode.ingredients[0]) // TODO: render all ingredient sets
            {
                int segments = graphNode.ingredients[0].Length + 1;
                float angle = (i + 1) * Mathf.PI / segments + Mathf.PI;
                RecipeNodeUI node = InstantiateNode(ingredient.item, new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r));
                ++i;
                CreateEdge(node.rectTransform.anchoredPosition, centerNode.rectTransform.anchoredPosition, edgeParent);
            }
        }

        public void ClearGraph()
        {
            foreach (Transform child in Content)
            {
                if (!ReferenceEquals(child, edgeParent))
                Destroy(child.gameObject);
            }

            foreach (Transform child in edgeParent)
            {
                Destroy(child.gameObject);
            }
        }

        private RecipeNodeUI InstantiateNode(ItemType itemType, Vector2 position)
        {
            // Assumes the node pivot is (0.5, 0.5)
            RecipeNodeUI node = Instantiate(graphNodePrefab);
            node.transform.position = position;
            node.transform.SetParent(Content, false);
            node.ItemType = itemType;
            node.graph = this;
            return node;
        }

        private void CreateEdge(Vector2 start, Vector2 end, Transform parent)
        {
            // assumes the Edge pivot is (0.5, 0)
            int width = 8;
            Vector2 delta = end - start;
            float zAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg - 90;
            Image line = Instantiate(directedLinePrefab, Vector3.zero, Quaternion.Euler(0, 0, zAngle), parent);

            // add correction offset
            line.rectTransform.anchoredPosition = start;
            line.rectTransform.sizeDelta = new Vector2(width, Mathf.FloorToInt(delta.magnitude / width) * width);
        }
#endregion

        #region RecipeMenu
        private RecipeMenuItem InstantiateMenuItem(ItemType itemType)
        {
            RecipeMenuItem menuItem = Instantiate(recipeMenuItemPrefab, 
                                                  Vector3.zero, 
                                                  Quaternion.identity, 
                                                  menuContent);
            menuItem.ItemType = itemType;
            menuItem.graph = this;
            menuItems.Add(itemType, menuItem);
            return menuItem;
        }

        private void RenderMenu()
        {
            foreach (ItemType item in RecipeGraph.Keys)
            {
                InstantiateMenuItem(item);
            }
        }
        #endregion
    }
}
