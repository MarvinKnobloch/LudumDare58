using System;
using Tower;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public static event Action<int> soulsChanged;

    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int soulsStartAmount;
    private int currentSouls;
    [SerializeField] private int defaultTowerCosts;
    public TowerRecipe[] towerRecipes;

    [Header("Slots")]
    [SerializeField] private int accessoiresSlotCosts;
    [SerializeField] private int headSlotCosts;
    [SerializeField] private int armSlotCosts;
    [SerializeField] private int bodySlotCosts;

    [NonSerialized] public Controls controls;

    [Header("TowerStuff")]
    [SerializeField] private int lifeStealChance = 5;
    [SerializeField] private int doubleDamageChance = 25;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        controls = Keybindinputmanager.Controls;
    }
    private void Start()
    {
        currentHealth = maxHealth;
        UpdateSouls(soulsStartAmount);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        IngameController.Instance.playerUI.HealthUIUpdate(currentHealth, maxHealth);
    }
    public void Heal(int amount)
    {
        currentHealth += amount;
        IngameController.Instance.playerUI.HealthUIUpdate(currentHealth, maxHealth);
    }
    public void UpdateSouls(int amount)
    {
        currentSouls += amount;
        IngameController.Instance.playerUI.SoulsUpdate(currentSouls);
        soulsChanged?.Invoke(currentSouls);
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
    public int GetCurrentSouls() => currentSouls;
    public int GetTowerCosts() => defaultTowerCosts;

    public int GetAccessoiresCosts() => accessoiresSlotCosts;
    public int GetArmsCosts() => armSlotCosts;
    public int GetHeadCosts() => headSlotCosts;
    public int GetBodyCosts() => bodySlotCosts;
    public int GetLifeStealChance() => lifeStealChance;
    public int GetDoubleDamageChance() => doubleDamageChance;
  }
