using System;
using System.Collections.Generic;
using TMPro;
using Tower;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<BodyObject, InventoryInfo> resources = new Dictionary<BodyObject, InventoryInfo>();

    private List<InventorySlot> slots = new List<InventorySlot>();
    [SerializeField] private GameObject prefabSlot;
    [SerializeField] private int slotAmount = 28;

    [SerializeField] private GameObject InventoryGrid;

    [Header("Equip Items")]
    public GameObject dragImage;
    public GameObject bodySlotsUI;
    [SerializeField] private BodySlots accessoiresSlot;
    [SerializeField] private BodySlots headSlot;
    [SerializeField] private BodySlots armsSlot;
    [SerializeField] private BodySlots bodySlot;
    [SerializeField] private BodySlots weaponSlot;
    public RangeIndicator rangeIndicator;

    [HideInInspector] public BodySlots currentBodySlots;
    [HideInInspector] public TowerBase currentSelectedTower;
    public BodyObject currentWeaponSlot;

    [Space]
    [SerializeField] private UpgradeTowerButton upgradeTowerButton;

    [Space]
    [SerializeField] private TowerInfo towerInfo;
    [HideInInspector] public bool canRecycle;

    [Header("Recipes")]
    [SerializeField] private RecipeUI recipeUI;
    private TowerRecipeSlot towerRecipeSlot;
    private TowerRecipe currentRecipe;

    private void Awake()
    {
        for (int i = 0; i < slotAmount; i++)
        {
            CreateNewSlot();
        }
        bodySlotsUI.SetActive(false);
    }
    private void Start()
    {
        accessoiresSlot.unlockObj.GetComponentInChildren<TextMeshProUGUI>().text = Player.Instance.GetAccessoiresCosts().ToString();
        headSlot.unlockObj.GetComponentInChildren<TextMeshProUGUI>().text = Player.Instance.GetHeadCosts().ToString();
        armsSlot.unlockObj.GetComponentInChildren<TextMeshProUGUI>().text = Player.Instance.GetArmsCosts().ToString();
        bodySlot.unlockObj.GetComponentInChildren<TextMeshProUGUI>().text = Player.Instance.GetBodyCosts().ToString();
    }

    public void AddResource(BodyObject bodyObject, int amount)
    {
        if (resources.ContainsKey(bodyObject) == false)
        {
            resources.Add(bodyObject, new InventoryInfo()
            {
                slotPosition = GetEmptySlot(),
                slotAmount = amount
            });

            slots[resources[bodyObject].slotPosition].bodyObject = bodyObject;
            slots[resources[bodyObject].slotPosition].SetValues(amount, bodyObject.Sprite);
        }

        else
        {
            resources[bodyObject].slotAmount += amount;
            slots[resources[bodyObject].slotPosition].SetValues(amount, bodyObject.Sprite);

            if (resources[bodyObject].slotAmount <= 0)
            {
                int position = resources[bodyObject].slotPosition;

                slots.Remove(slots[resources[bodyObject].slotPosition]);
                slots.Add(slots[resources[bodyObject].slotPosition]);
                resources.Remove(bodyObject);

                foreach (InventoryInfo item in resources.Values)
                {
                    if(item.slotPosition > position)
                    {
                        item.slotPosition -= 1;
                    }
                }
            }
        }
    }
    private int GetEmptySlot()
    {
        for(int i = 0;i < slots.Count; i++)
        {
            if (slots[i].slotIsFull == false)
            {
                slots[i].slotIsFull = true;
                return i;
            }
        }
        Debug.Log("No more empty slots");
        CreateNewSlot();
        return slots.Count - 1;
    }
    private void CreateNewSlot()
    {
        InventorySlot slot = Instantiate(prefabSlot, Vector3.zero, Quaternion.identity, InventoryGrid.transform).GetComponent<InventorySlot>();
        slots.Add(slot);
        slot.inventory = this;
        slot.HideText();
    }
    public void SetCurrentTower(TowerBase tower)
    {
        currentSelectedTower = tower;
        if (currentSelectedTower.isRecipeTower == false)
        {
            SetSlots(accessoiresSlot, currentSelectedTower.currentAccessoires);
            SetSlots(headSlot, currentSelectedTower.currentHead);
            SetSlots(armsSlot, currentSelectedTower.currentArms);
            SetSlots(bodySlot, currentSelectedTower.currentBody);

            SetSlots(weaponSlot, currentSelectedTower.currentWeapon);
            currentWeaponSlot = currentSelectedTower.currentWeapon;
            weaponSlot.gameObject.SetActive(true);
        }
        else
        {
            if (currentSelectedTower.accessoiresSlotUnlocked) SetSlots(accessoiresSlot, currentSelectedTower.currentAccessoires);
            else SetLockedState(accessoiresSlot);
            if (currentSelectedTower.headSlotUnlocked) SetSlots(headSlot, currentSelectedTower.currentHead);
            else SetLockedState(headSlot);
            if (currentSelectedTower.armsSlotUnlocked) SetSlots(armsSlot, currentSelectedTower.currentArms);
            else SetLockedState(armsSlot);
            if (currentSelectedTower.bodySlotUnlocked) SetSlots(bodySlot, currentSelectedTower.currentBody);
            else SetLockedState(bodySlot);

            SetLockedState(weaponSlot);
            currentWeaponSlot = null;
            weaponSlot.gameObject.SetActive(false);
        }

        SetUpgradeTowerButton();
        SetRangeIndicator();
        SetTowerInfo();

        bodySlotsUI.SetActive(true);
    }
    public void DeselectTower()
    {
        DisableSlot(accessoiresSlot);
        DisableSlot(headSlot);
        DisableSlot(armsSlot);
        DisableSlot(bodySlot);
        DisableSlot(weaponSlot);

        DisableRangeIndicator();
        currentSelectedTower = null;
        towerInfo.TowerInfoUpdate();
    }
    private void DisableSlot(BodySlots bodySlots)
    {
        bodySlots.gameObject.SetActive(false);
        bodySlots.enabled = false;
        bodySlots.ClearSlot();
    }
    private void SetSlots(BodySlots bodySlots , BodyObject bodyObject)
    {
        bodySlots.gameObject.SetActive(true);
        bodySlots.unlockObj.SetActive(false);
        bodySlots.enabled = true;

        if (bodyObject != null)
        {
            bodySlots.UpdateSlot(bodyObject);
        }
        else
        {
            bodySlots.ClearSlot();
        }
    }
    private void SetLockedState(BodySlots bodySlots)
    {
        bodySlots.unlockObj.SetActive(true);
        bodySlots.enabled = false;
        bodySlots.ClearSlot();
    }
    public void UnlockSlot(BodySlots bodySlot)
    {
        int currentSouls = Player.Instance.GetCurrentSouls();
        int costs = 0;

        switch (bodySlot.bodyPart)
        {
            case BodyPart.Accessory:
                costs = Player.Instance.GetAccessoiresCosts();
                if (currentSouls < costs) return;
                currentSelectedTower.accessoiresSlotUnlocked = true;
                break;
            case BodyPart.Head:
                costs = Player.Instance.GetHeadCosts();
                if (currentSouls < costs) return;
                currentSelectedTower.headSlotUnlocked = true;
                break;
            case BodyPart.Arm:
                costs = Player.Instance.GetArmsCosts();
                if (currentSouls < costs) return;
                currentSelectedTower.armsSlotUnlocked = true;
                break;
            case BodyPart.Torso:
                costs = Player.Instance.GetBodyCosts();
                if (currentSouls < costs) return;
                currentSelectedTower.bodySlotUnlocked = true;
                break;
            case BodyPart.Weapon:
                break;
        }

        bodySlot.unlockObj.SetActive(false);
        bodySlot.enabled = true;
        Player.Instance.UpdateSouls(-costs);
    }

    public void SetRangeIndicator()
    {
        rangeIndicator.gameObject.transform.position = currentSelectedTower.gameObject.transform.position;
        rangeIndicator.gameObject.SetActive(true);
        rangeIndicator.DrawCircle(currentSelectedTower.GetTowerRange());
    }
    public void DisableRangeIndicator()
    {
        rangeIndicator.gameObject.SetActive(false);
    }

    public void SetUpgradeTowerButton()
    {
        if (currentSelectedTower.isRecipeTower)
        {
            upgradeTowerButton.gameObject.SetActive(true);
            upgradeTowerButton.UpdateUpgradeTowerInfo(false, 100);
        }
        else
        {
            upgradeTowerButton.gameObject.SetActive(true);

            if (currentSelectedTower.currentRecipe == null) upgradeTowerButton.UpdateUpgradeTowerInfo(false, currentSelectedTower.recipeMatchPercent);
            else upgradeTowerButton.UpdateUpgradeTowerInfo(true, currentSelectedTower.recipeMatchPercent);
        }
    }
    public void SetTowerInfo()
    {
        towerInfo.TowerInfoUpdate();
    }

    public void CheckForRecipeUIUpdate(BodyPart bodyPart)
    {
        currentRecipe = null;
        //Loop through all recipes that needed to be updated
        for (int i = 0; i < currentWeaponSlot.mainPartOfRecipe.Length; i++)
        {
            towerRecipeSlot = null;
            currentRecipe = currentWeaponSlot.mainPartOfRecipe[i];

            //Get the repice slot form the UI
            for (int t = 0; t < recipeUI.recipes.Count; t++)
            {
                if(recipeUI.recipes[t].towerRecipe == currentRecipe)
                {
                    towerRecipeSlot = recipeUI.recipes[t];
                    break;
                }
            }
            if (towerRecipeSlot == null) break;

            //Only check for the body part that is update, if the weapon is changed need to check all slots.
            switch (bodyPart)
            {
                case BodyPart.Accessory:
                    CheckRecipeSlots(i, accessoiresSlot);
                    break;
                case BodyPart.Head:
                    CheckRecipeSlots(i, headSlot);
                    break;
                case BodyPart.Arm:
                    CheckRecipeSlots(i, armsSlot);
                    break;
                case BodyPart.Torso:
                    CheckRecipeSlots(i, bodySlot);
                    break;
                case BodyPart.Weapon:
                    CheckRecipeSlots(i, accessoiresSlot);
                    CheckRecipeSlots(i, headSlot);
                    CheckRecipeSlots(i, armsSlot);
                    CheckRecipeSlots(i, bodySlot);
                    break;
            }
        }
    }
    private void CheckRecipeSlots(int number, BodySlots bodySlot)
    {
        //After adding the new item to the tower, check if the recipe contains the item
        if (currentRecipe.Recipe.Contains(bodySlot.bodyObject))
        {
            //Get the position of the item and update the Recipe UI
            int postion = currentRecipe.Recipe.IndexOf(bodySlot.bodyObject);
            currentRecipe.partUnlocked[postion] = true;
            towerRecipeSlot.UpdateSlots();
        }
    }

    [Serializable]
    public class InventoryInfo
    {
        public int slotPosition;
        public int slotAmount;
    }

}
