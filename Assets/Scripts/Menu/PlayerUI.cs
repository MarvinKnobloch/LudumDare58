using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
    private Controls controls;

    [Header("Value")]
    [SerializeField] private Image healthbar;
    //[SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI soulsText;

    [Header("DialogBox")]
    public GameObject dialogBox;

    [Header("Inventory")]
    public Inventory inventory;

    [Header("ToolTip")]
    [field: SerializeField] public GameObject itmeTooltipWindow { get; private set; }
    [field: SerializeField] public TextMeshProUGUI itmeTooltipText { get; private set; }
    [field: SerializeField] public GameObject statsTooltipWindow { get; private set; }
    [field: SerializeField] public TextMeshProUGUI statsTooltipText { get; private set; }


    [Header("Other")]
    public GameObject startNextLevelButton;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    public void HealthUIUpdate(int current, int max)
    {
        healthbar.fillAmount = (float)current / max;
       // healthText.text = current + "/" + max;
    }
    public void SoulsUpdate(int amount)
    {
        soulsText.text = amount.ToString();
    }
    public void ToggleTooltipWindow(bool toggle, Vector3 position, GameObject window)
    {
        if (toggle) 
        { 
            window.transform.position = position;
            window.SetActive(true); 
        }
        else window.SetActive(false);
    }
    public void StartNextLevel()
    {
        LevelManager.Instance.StartNextLevel();
    }
}
