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
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("DialogBox")]
    public GameObject dialogBox;

    [Header("TowerInfoMenu")]
    public TowerInfo towerInfoMenu;


    [Header("Inventory")]
    public Inventory inventory;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    public void HealthUIUpdate(int current, int max)
    {
        healthbar.fillAmount = (float)current / max;
        healthText.text = current + "/" + max;
    }
    public void ToggleTowerInfoMenu()
    {
        if (towerInfoMenu.gameObject.activeSelf == false) towerInfoMenu.gameObject.SetActive(true);
        else towerInfoMenu.gameObject.SetActive(false);
    }
}
