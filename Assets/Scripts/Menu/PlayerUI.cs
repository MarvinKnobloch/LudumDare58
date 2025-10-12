using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
    [Header("Value")]
    [SerializeField] private Image healthbar;
    //[SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI soulsText;

    [Header("DialogBox")]
    public GameObject dialogBox;

    [Header("Inventory")]
    public Inventory inventory;


    [field: SerializeField, Header("ToolTip")] public GameObject itemTooltipWindow { get; private set; }
    [field: SerializeField] public TextMeshProUGUI itmeTooltipText { get; private set; }
    [field: SerializeField] public GameObject statsTooltipWindow { get; private set; }
    [field: SerializeField] public TextMeshProUGUI statsTooltipText { get; private set; }

    [Header("GameOver")]
    [field: SerializeField] public GameObject gameOverScreen { get; private set; }

    [Header("Other")]
    [field: SerializeField] public GameObject startNextLevelButton { get; private set; }

    public void HealthUIUpdate(int current, int max)
    {
        healthbar.fillAmount = (float)current / max;
       // healthText.text = current + "/" + max;
    }
    public void SoulsUpdate(int amount)
    {
        soulsText.text = amount.ToString();
    }
    public void ToggleTooltipWindow(bool toggle, GameObject window)
    {
        if (toggle) 
        { 
            window.SetActive(true); 
        }
        else window.SetActive(false);
    }
    public void StartNextLevel()
    {
        LevelManager.Instance.StartNextLevel();
    }
}
