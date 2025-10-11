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
        playerUI.ToggleTooltipWindow(true, Utility.MousePostion(), playerUI.statsTooltipWindow);
        switch (statTypes)
        {
            case StatTypes.Attack:
                playerUI.statsTooltipText.text = "Attack";
                break;
            case StatTypes.Speed:
                playerUI.statsTooltipText.text = "Speed";
                break;
            case StatTypes.Range:
                playerUI.statsTooltipText.text = "Range";
                break;
            case StatTypes.Aoe:
                playerUI.statsTooltipText.text = "Area size";
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        playerUI.ToggleTooltipWindow(false, Vector3.zero, playerUI.statsTooltipWindow);
    }
}
