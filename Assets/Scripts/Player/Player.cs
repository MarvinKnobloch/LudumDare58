using System;
using Tower;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int currentSouls;
    [SerializeField] private int defaultTowerCosts;
    public TowerRecipe[] towerRecipes;

    [Header("Slots")]
    [SerializeField] private int accessoiresSlotCosts;
    [SerializeField] private int headSlotCosts;
    [SerializeField] private int armSlotCosts;
    [SerializeField] private int bodySlotCosts;

    [NonSerialized] public Controls controls;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        controls = Keybindinputmanager.Controls;
    }
    private void Start()
    {
        currentHealth = maxHealth;
        UpdateSouls(defaultTowerCosts);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        //Update Health UI
    }
    public void UpdateSouls(int amount)
    {
        currentSouls += amount;
        IngameController.Instance.playerUI.SoulsUpdate(currentSouls);
    }
    public bool CheckForTowerCosts()
    {
        if (currentSouls < defaultTowerCosts) return false;
        else return true;
    }
    public void BuyTower()
    {
        UpdateSouls(-defaultTowerCosts);
    }
    public int GetTowerCosts() => defaultTowerCosts;

    public int GetAccessoiresCosts() => accessoiresSlotCosts;
    public int GetArmsCosts() => armSlotCosts;
    public int GetHeadCosts() => headSlotCosts;
    public int GetBodyCosts() => bodySlotCosts;
  }
