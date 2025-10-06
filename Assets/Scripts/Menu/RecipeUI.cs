using Tower;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    [SerializeField] private GameObject recipeBackground;

    [Space]
    [SerializeField] private TowerRecipe[] allTowerRecipes;
    [SerializeField] private GameObject towerRepicePrefab;
    [SerializeField] private GameObject towerSlotsGrid;

    private void Awake()
    {
        RectTransform rectTransform = recipeBackground.GetComponent<RectTransform>();
        float bonusHeight = (towerRepicePrefab.GetComponent<RectTransform>().sizeDelta.y + towerSlotsGrid.GetComponent<GridLayoutGroup>().spacing.y) * allTowerRecipes.Length;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + bonusHeight);

        for (int i = 0; i < allTowerRecipes.Length; i++)
        {
            TowerRecipeSlot towerRecipeSlot = Instantiate(towerRepicePrefab, Vector3.zero, Quaternion.identity, towerSlotsGrid.transform).GetComponent<TowerRecipeSlot>();
            towerRecipeSlot.SetSlot(allTowerRecipes[i]);
        }
    }
    public void ToggleRepiceUI()
    {
        if(recipeBackground.activeSelf) recipeBackground.SetActive(false);
        else recipeBackground.SetActive(true);
    }
}
