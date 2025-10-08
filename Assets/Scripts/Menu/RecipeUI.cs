using System.Collections.Generic;
using Tower;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    [SerializeField] private GameObject recipeBackground;

    [Space]
    [SerializeField] private GameObject towerRepicePrefab;
    [SerializeField] private GameObject towerSlotsGrid;

    private List<TowerRecipeSlot> recipes = new List<TowerRecipeSlot>();


    private void OnEnable()
    {
        TowerBase.UpdateRecipesUI += UpdateRecipeUI;
    }
    private void OnDisable()
    {
        TowerBase.UpdateRecipesUI -= UpdateRecipeUI;
    }
    private void Start()
    {
        RectTransform rectTransform = recipeBackground.GetComponent<RectTransform>();
        float bonusHeight = 
            (towerRepicePrefab.GetComponent<RectTransform>().sizeDelta.y + towerSlotsGrid.GetComponent<GridLayoutGroup>().spacing.y) * Player.Instance.towerRecipes.Length;

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + bonusHeight);

        for (int i = 0; i < Player.Instance.towerRecipes.Length; i++)
        {
            TowerRecipeSlot towerRecipeSlot = Instantiate(towerRepicePrefab, Vector3.zero, Quaternion.identity, towerSlotsGrid.transform).GetComponent<TowerRecipeSlot>();
            recipes.Add(towerRecipeSlot);
            towerRecipeSlot.SetSlot(Player.Instance.towerRecipes[i]);
        }
    }
    public void UpdateRecipeUI()
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            recipes[i].SetSlot(Player.Instance.towerRecipes[i]);
        }
    }
    public void ToggleRepiceUI()
    {
        if(recipeBackground.activeSelf) recipeBackground.SetActive(false);
        else recipeBackground.SetActive(true);
    }
}
