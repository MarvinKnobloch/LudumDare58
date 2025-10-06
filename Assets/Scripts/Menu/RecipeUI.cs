using Tower;
using UnityEngine;

public class RecipeUI : MonoBehaviour
{
    [SerializeField] private GameObject recipeBackground;

    [Space]
    [SerializeField] private TowerRecipe[] allTowerRecipes;
    [SerializeField] private GameObject towerRepicePrefab;
    [SerializeField] private GameObject towerSlotsGrid;

    private void Awake()
    {
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
