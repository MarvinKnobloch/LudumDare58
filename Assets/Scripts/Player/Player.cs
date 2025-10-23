using System;
using Marvin.AudioSystem;
using Tower;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public static event Action<int> soulsChanged;

    private Controls controls;

    [SerializeField] private int maxHealth = 30;
    [SerializeField] private int currentHealth;
    [SerializeField] private int soulsStartAmount = 90;
    private int currentSouls;
    [SerializeField] private int defaultTowerCosts = 50;
    [SerializeField] private int recycleSoulsAmount = 1;
    public TowerRecipe[] towerRecipes;

    [Header("Slots")]
    [SerializeField] private int accessoiresSlotCosts;
    [SerializeField] private int headSlotCosts;
    [SerializeField] private int armSlotCosts;
    [SerializeField] private int bodySlotCosts;

    [Header("TowerStuff")]
    [SerializeField] private int lifeStealChance = 5;
    [SerializeField] private int doubleDamageChance = 25;

    [Space]
    [SerializeField] private AudioObj takeDamageSound;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        controls = new Controls();
    }
    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Cancel.performed += DeselectTowerHotkey;
    }
    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Cancel.performed -= DeselectTowerHotkey;
    }
    private void Start()
    {
        currentHealth = maxHealth;
        UpdateSouls(soulsStartAmount);

        IngameController.Instance.playerUI.HealthUIUpdate(currentHealth, maxHealth);

        ResetRecipesUnlockState();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        IngameController.Instance.playerUI.HealthUIUpdate(currentHealth, maxHealth);

        AudioManager.Instance.PlayAudioObjOneShot(takeDamageSound);

        if (currentHealth <= 0)
        {
            IngameController.Instance.playerUI.gameOverScreen.SetActive(true);
        }
    }
    public void Heal(int amount)
    {
        if (currentHealth >= maxHealth) return;

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
    private void ResetRecipesUnlockState()
    {
        if (GameManager.Instance.resetRecipeObjects == false) return;

        for (int i = 0; i < towerRecipes.Length; i++)
        {
            towerRecipes[i].partUnlocked.Clear();
            for (int t = 0; t < towerRecipes[i].defaultUnlockState.Count; t++)
            {
                towerRecipes[i].partUnlocked.Add(false);
                if (towerRecipes[i].defaultUnlockState[t] == true)
                {
                    towerRecipes[i].partUnlocked[t] = true;
                }
                else towerRecipes[i].partUnlocked[t] = false;
            }

        }
    }
    private void DeselectTowerHotkey(InputAction.CallbackContext context)
    {
        if (IngameController.Instance.playerUI.inventory.dragImage.activeSelf == true) return;
        IngameController.Instance.playerUI.inventory.DeselectTower();
    }
    public int GetCurrentSouls() => currentSouls;
    public int GetTowerCosts() => defaultTowerCosts;

    public int GetAccessoiresCosts() => accessoiresSlotCosts;
    public int GetArmsCosts() => armSlotCosts;
    public int GetHeadCosts() => headSlotCosts;
    public int GetBodyCosts() => bodySlotCosts;
    public int GetLifeStealChance() => lifeStealChance;
    public int GetDoubleDamageChance() => doubleDamageChance;
    public int GetRecycleSoulsAmount() => recycleSoulsAmount;
  }
