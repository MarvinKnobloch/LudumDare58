using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
    private Controls controls;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionField;
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Value")]
    [SerializeField] private Image healthbar;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("DialogBox")]
    public GameObject dialogBox;

    [Header("TowerInfoMenu")]
    public TowerInfo towerInfoMenu;


    [Header("Inventory")]
    public Inventory inventory;


    private float timer;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    private void Start()
    {
        StartCoroutine(InteractionFieldDisable());
    }
    IEnumerator InteractionFieldDisable()
    {
        yield return null;
        interactionField.SetActive(false);
        interactionField.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }
    public void HandleInteractionBox(bool state)
    {
        if(interactionField != null) interactionField.SetActive(state);
    }
    public void InteractionTextUpdate(string text)
    {
        interactionText.text = text + " (<color=green>" + controls.Player.Interact.GetBindingDisplayString() + "</color>)";
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
