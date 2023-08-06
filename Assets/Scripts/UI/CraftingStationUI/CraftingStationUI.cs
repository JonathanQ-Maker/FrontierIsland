

using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace FrontierIsland
{
    public class CraftingStationUI : WorldUI
    {
        [SerializeField]
        private CraftingStationItem stationItemPrefab;
        [SerializeField]
        private TextMeshProUGUI title, description;
        [SerializeField]
        private RectTransform selectOverlay, recipeContent;


        private ICraftingStation station;
        public ICraftingStation Station 
        {
            get { return station; }
            set
            {
                station = value;
                focus = station.transform;
            }
        }

        private CraftingStationItem[] stationItems;

        private int selectIndex;
        public int SelectIndex 
        { 
            get { return selectIndex; }
            set 
            { 
                selectIndex = value;
                UpdateSelectRecipe();
            }
        }

        public const int CAMERA_CONTROL_MASK = 1 << 1;
        public bool Active
        {
            get { return gameObject.activeSelf; }
            set 
            {
                gameObject.SetActive(value);
            }
        }

        public void UpdateSelectRecipe()
        {
            if (stationItems == null) return;
            selectOverlay.SetParent(stationItems[SelectIndex].transform, false);
            description.text = GetRecipeDescription(station.Recipes[SelectIndex]);
        }

        private void Start()
        {
            Render();
            UpdateSelectRecipe();
        }


        /// <summary>
        /// Renders the <see cref="CraftingStationUI"/>
        /// </summary>
        private void Render()
        {
            if (stationItems != null) ClearStationItems();
            title.text = station.Title;

            if (stationItems == null || stationItems.Length != station.Recipes.Length)
            {
                stationItems = new CraftingStationItem[station.Recipes.Length];
            }

            for (int i = 0; i < station.Recipes.Length; ++i)
            {
                CraftingStationItem stationItem = Instantiate(stationItemPrefab,
                                                      Vector3.zero,
                                                      Quaternion.identity,
                                                      recipeContent);
                stationItem.stationUI = this;
                stationItem.Load(i);
                stationItems[i] = stationItem;
            }

            selectOverlay.gameObject.SetActive(stationItems.Length > 0);
        }

        private void ClearStationItems()
        {
            if (stationItems == null) return;

            for (int i = 0; i < stationItems.Length; ++i)
            {
                Destroy(stationItems[i].gameObject);
            }
        }

        private string GetRecipeDescription(ItemRecipe recipe)
        {
            StringBuilder builder = new StringBuilder(recipe.Ingredients.Length);
            for (int i = 0; i < recipe.Ingredients.Length; ++i)
            {
                ItemStack ingredient = ItemAtlas.Get(recipe.Ingredients[i]);
                builder.AppendLine($"- {ingredient.name} <color=#9F0909>(0/1)</color>");
            }
            
            ItemStack result = ItemAtlas.Get(recipe.ResultItem);
            string description = $"<b>{result.name} (1)</b>\n" +
                $"<color=#898989><i>{result.description}</i></color>\n\n"+
                $"{builder}";
            return description;
        }
    }
}
