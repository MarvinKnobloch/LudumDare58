using System.Collections.Generic;
using Tower;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    private Controls controls;
    [SerializeField] private GameObject recipeBackground;

    [Space]
    [SerializeField] private GameObject towerRepicePrefab;
    [SerializeField] private GameObject towerSlotsGrid;

    private List<TowerRecipeSlot> recipes = new List<TowerRecipeSlot>();

    private void Awake()
    {
        controls = new Controls();
    }

    private void OnEnable()
    {
        controls.Enable();

        TowerBase.UpdateRecipesUI += UpdateRecipeUI;
        controls.Player.RecipeMenu.performed += ToggleHotkey;
    }
    private void OnDisable()
    {
        controls.Player.RecipeMenu.performed -= ToggleHotkey;
        TowerBase.UpdateRecipesUI -= UpdateRecipeUI;

        controls.Disable();
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

    public void ToggleHotkey(InputAction.CallbackContext context)
    {
        if (IngameController.Instance.menuController.gameIsPaused) return;

        ToggleRepiceUI();
    }
}
