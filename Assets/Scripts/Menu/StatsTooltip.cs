using System.Collections;
using TMPro;
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
                playerUI.statsTooltipText.text = "<u><b>Damage:</u></b>\n\nItem Damage x Tower Attack Scaling (<color=green>" + scalingText + "%</color>)";
                break;
            case StatTypes.Speed:
                playerUI.statsTooltipText.text = "<u><b>Attack Speed</u></b>";
                break;
            case StatTypes.Range:
                if (playerUI.inventory.currentSelectedTower != null) scalingText = playerUI.inventory.currentSelectedTower.GetRangeScaling().ToString();
                playerUI.statsTooltipText.text = "<u><b>Range:</u></b>\n\nItem Range x Tower Range Scaling (<color=green>" + scalingText + "%</color>)";
                break;
            case StatTypes.Aoe:
                playerUI.statsTooltipText.text = "<u><b>Area Size:</u></b>\n\nIf negative the tower will damage only one enemy.";
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        playerUI.ToggleTooltipWindow(false, playerUI.statsTooltipWindow);
    }
}
