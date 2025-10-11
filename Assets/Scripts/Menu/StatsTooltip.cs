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
        SetWindowPosition();
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

    public void SetWindowPosition()
    {
        playerUI.ToggleTooltipWindow(true, playerUI.statsTooltipWindow);
        //Braucht ein Frame um die Height vom ContenSizeFitter zu setzen. Scale wird für den einen Frame auf 0 gesetzt damit dann das FEnster nicht springen sieht
        //StartCoroutine(SetWindowPostionAfterResize());
    }
    IEnumerator SetWindowPostionAfterResize()
    {
        yield return null;
        GameObject window = playerUI.statsTooltipWindow;
        playerUI.ToggleTooltipWindow(true, window);
        ////window.transform.localScale = new Vector3(1, 1, 1);
        ////Ist keine Formel, einfach ein bisschen ausprobiert was passt
        //float widthOffset = Screen.width / 9.5f;  //7

        //float heigthoffset = ((Screen.height * 0.5f) - Input.mousePosition.y) / 70;              //Bestimmt ob das Fenster nach unten oder oben geht.

        ////if (heigthoffset > 0) heigthoffset += tooltipRect.rect.height * rectHeightMultiplier;    //Wie weit das Fenster nach unten/oben geht basierend auf der Größe
        ////else heigthoffset -= tooltipRect.rect.height * rectHeightMultiplier;

        ////links oder rechts von mousePosition          1.35f = 35% vom rechten Bildschirmrand
        //if (Screen.width / Input.mousePosition.x > 1.43f) window.transform.position = Input.mousePosition + new Vector3(widthOffset, heigthoffset, 0);
        //else window.transform.position = Input.mousePosition + new Vector3(widthOffset * -1, heigthoffset, 0);
    }
}
