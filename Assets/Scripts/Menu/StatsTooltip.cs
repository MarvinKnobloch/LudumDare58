using System.Collections;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatsTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private StatTypes statTypes;
    private PlayerUI playerUI;

    private void Start()
    {
        playerUI = IngameController.Instance.playerUI;
    }
    public enum StatTypes
    {
        Attack,
        Speed,
        Range,
        Aoe,
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        playerUI.ToggleTooltipWindow(true, playerUI.statsTooltipWindow);
        string scalingText = "???";
        switch (statTypes)
        {
            case StatTypes.Attack:
                if (playerUI.inventory.currentSelectedTower != null) scalingText = playerUI.inventory.currentSelectedTower.GetDamageScaling().ToString();
                playerUI.statsTooltipText.text = "Items Attack + tower attack scaling(<color=green>" + scalingText + "</color>)\n = overall damage";
                break;
            case StatTypes.Speed:
                playerUI.statsTooltipText.text = "Tower attack speed";
                break;
            case StatTypes.Range:
                if (playerUI.inventory.currentSelectedTower != null) scalingText = playerUI.inventory.currentSelectedTower.GetRangeScaling().ToString();
                playerUI.statsTooltipText.text = "Items Range + tower range scaling(<color=green>" + scalingText + "</color>)\n = overall range";
                break;
            case StatTypes.Aoe:
                playerUI.statsTooltipText.text = "Area size, if negative the tower will damage only one enemy.";
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        playerUI.ToggleTooltipWindow(false, playerUI.statsTooltipWindow);
    }
}
