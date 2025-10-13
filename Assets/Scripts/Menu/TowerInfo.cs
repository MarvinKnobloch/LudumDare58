using TMPro;
using Tower;
using UnityEngine;

public class TowerInfo : MonoBehaviour
{
    [SerializeField] private GameObject scrollClosed;
    [SerializeField] private GameObject scrollOpened;

    [SerializeField] private TextMeshProUGUI numberText;
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
            numberText.text = string.Empty;
            specialText.text = string.Empty;
        }
    }
    public void TowerInfoUpdate()
    {
        TowerBase tower = IngameController.Instance.playerUI.inventory.currentSelectedTower;
        if (tower == null)
        {
            numberText.text = string.Empty;
            specialText.text = string.Empty;
        }
        else
        {
            numberText.text = tower.finalDamage + "\n" + tower.finalAttackSpeed + "\n" + tower.finalRange + "\n" + tower._currentAoeRadius;
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
}
