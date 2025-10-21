using TMPro;
using Tower;
using UnityEngine;

public class TowerInfo : MonoBehaviour
{
    [SerializeField] private GameObject scrollClosed;
    [SerializeField] private GameObject scrollOpened;

    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI aoeText;
    [SerializeField] private TextMeshProUGUI specialText;


    public void ToggleScrolls()
    {
        if (scrollClosed.activeSelf)
        {
            scrollClosed.SetActive(false);
            scrollOpened.SetActive(true);
            TowerInfoUpdate();
        }
        else
        {
            scrollClosed.SetActive(true);
            scrollOpened.SetActive(false);

            ClearText();
        }
    }
    public void TowerInfoUpdate()
    {
        TowerBase tower = IngameController.Instance.playerUI.inventory.currentSelectedTower;
        if (tower == null)
        {
            ClearText();
        }
        else
        {
            damageText.text = tower.finalDamage.ToString();
            attackSpeedText.text = tower.finalAttackSpeed.ToString();
            rangeText.text = tower.finalRange.ToString();
            aoeText.text = tower.currentAoeRadius.ToString();

            //if(tower.currentAoeRadius >= 0) aoeText.text = "<color=green>" + tower.currentAoeRadius + "</color>";
            //else aoeText.text = "<color=red>" + tower.currentAoeRadius + "</color>";

            specialText.text = string.Empty;

            int values = 0;
            if (tower.GetSlow() > 0) 
            {
                specialText.text += "Slow (<color=green>" + tower.GetSlow().ToString() + "%</color>), "; 
                values++;
            }
            if(tower.GetLifesteal() == true)
            {
                specialText.text += "Lifesteal, ";
                values++;
            }
            if (tower.GetDoubleDamage() == true)
            {
                if (values > 0) specialText.text += "\n";
                specialText.text += "Double Damage (<color=green>" + Player.Instance.GetDoubleDamageChance() + "%</color>), ";
                values++;
            }
            if(tower.GetAdditionalProjectiles() > 0)
            {
                if (values > 0) specialText.text += "\n";
                if (tower.GetAdditionalProjectiles() == 1) specialText.text += "<color=green>+" + tower.GetAdditionalProjectiles() + "</color> Projectile";
                else specialText.text += "<color=green>+" + tower.GetAdditionalProjectiles() + "</color> Projectiles";

            }
        }
    }
    private void ClearText()
    {
        damageText.text = string.Empty;
        attackSpeedText.text = string.Empty;
        rangeText.text = string.Empty;
        aoeText.text = string.Empty;
        specialText.text = string.Empty;
    }
}
