using System.Collections;
using System.Collections.Generic;
using Tower;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    private Controls controls;
    [SerializeField] private GameObject recipeBackground;

    [Space]
    [SerializeField] private Transform openPositionTransform;
    [SerializeField] private float moveTime;
    private Transform uiOpenPosition;
    private Vector3 uiClosedPosition;
    private Vector3 endPosition;
    private bool isOpen;
    private float timer;

    [Space]
    [SerializeField] private GameObject towerRepicePrefab;
    [SerializeField] private GameObject towerSlotsGrid;

    private List<TowerRecipeSlot> recipes = new List<TowerRecipeSlot>();

    private void Awake()
    {
        controls = new Controls();
        uiClosedPosition = transform.position;
        uiOpenPosition = openPositionTransform;
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
        //RectTransform rectTransform = recipeBackground.GetComponent<RectTransform>();
        //float bonusHeight = 
        //    (towerRepicePrefab.GetComponent<RectTransform>().sizeDelta.y + towerSlotsGrid.GetComponent<GridLayoutGroup>().spacing.y) * Player.Instance.towerRecipes.Length;

        //rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + bonusHeight);

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
    public void ToggleHotkey(InputAction.CallbackContext context)
    {
        if (IngameController.Instance.menuController.gameIsPaused) return;

        ToggleRepiceUI();
    }
    public void ToggleRepiceUI()
    {
        StopAllCoroutines();
        timer = 0;

        if (isOpen == false) endPosition = uiOpenPosition.position;
        else endPosition = uiClosedPosition;

        isOpen = !isOpen;
        StartCoroutine(MoveUI());

    }
    IEnumerator MoveUI()
    {
        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, endPosition, timer / moveTime);
            yield return null;
        }

    }

}
