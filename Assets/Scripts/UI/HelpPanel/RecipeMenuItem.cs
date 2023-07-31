using UnityEngine.EventSystems;

namespace FrontierIsland
{
    public class RecipeMenuItem : RecipeGraphItemUI
    {
        private ItemType itemType;
        public ItemType ItemType
        {
            get { return itemType; }
            set
            {
                itemType = value;
                ItemStack item = ItemAtlas.Get(itemType);
                Load(item.GetIcon(), item.GetToolTip());
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            graph.RenderGraph(ItemType);
        }
    }
}
