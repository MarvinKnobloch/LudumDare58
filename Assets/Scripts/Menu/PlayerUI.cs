using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Collections;
using Tower;

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

    [Header("Tutorial")]
    public Tutorial tutorial;




    [field: SerializeField, Header("ToolTip")] public GameObject itemTooltipWindow { get; private set; }
    [field: SerializeField] public TextMeshProUGUI itmeTooltipText { get; private set; }
    [field: SerializeField] public GameObject statsTooltipWindow { get; private set; }
    [field: SerializeField] public TextMeshProUGUI statsTooltipText { get; private set; }


    [field: SerializeField,Header("Other")] public NextLevelButton startNextLevelButton { get; private set; }
    [field: SerializeField] public GameObject gameOverScreen { get; private set; }
    [field: SerializeField] public GameObject victoryScreen { get; private set; }


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
    public void SetToolTipWindow(BodyObject bodyObject)
    {
        if (bodyObject == null) return;

        ToggleTooltipWindow(true, itemTooltipWindow);

        TextMeshProUGUI itemText = itmeTooltipText;
        itemText.text = string.Empty;
        itemText.text += "<u><b>" + bodyObject.Name + "</u></b>\n\n";
        bool baseStat = false;
        bool towerStat = false;

        if (bodyObject.BonusDamage != 0) baseStat = true;
        if (bodyObject.BonusDamage > 0) itemText.text += "<color=green>+" + bodyObject.BonusDamage + "</color> Damage\n";
        else if (bodyObject.BonusDamage < 0) itemText.text += "<color=red>" + bodyObject.BonusDamage + "</color> Damage\n";

        if (bodyObject.BonusAttackSpeed != 0) baseStat = true;
        if (bodyObject.BonusAttackSpeed > 0) itemText.text += "<color=green>+" + bodyObject.BonusAttackSpeed + "%</color> Attack Speed\n";
        else if (bodyObject.BonusAttackSpeed < 0) itemText.text += "<color=red>" + bodyObject.BonusAttackSpeed + "%</color> Attack Speed\n";

        if (bodyObject.BonusRange != 0) baseStat = true;
        if (bodyObject.BonusRange > 0) itemText.text += "<color=green>+" + bodyObject.BonusRange + "</color> Range\n";
        else if (bodyObject.BonusRange < 0) itemText.text += "<color=red>" + bodyObject.BonusRange + "</color> Range\n";

        if (bodyObject.BonusAoeRadius != 0) baseStat = true;
        if (bodyObject.BonusAoeRadius > 0) itemText.text += "<color=green>+" + bodyObject.BonusAoeRadius + "</color> Area Size\n";
        else if (bodyObject.BonusAoeRadius < 0) itemText.text += "<color=red>" + bodyObject.BonusAoeRadius + "</color> Area Size\n";
        if (baseStat == true) itemText.text += "\n";

        if (bodyObject.DamageScalingPercentage > 0)
        {
            towerStat = true;
            itemText.text += "<color=green>" + bodyObject.DamageScalingPercentage + "%</color> Damage Scaling\n";
        }

        if (bodyObject.BaseAttackSpeed > 0)
        {
            towerStat = true;
            itemText.text += "<color=green>" + bodyObject.BaseAttackSpeed + "</color> Base Attack Speed\n";
        }

        if (bodyObject.RangeScalingPercentage > 0)
        {
            towerStat = true;
            itemText.text += "<color=green>" + bodyObject.RangeScalingPercentage + "%</color> Range Scaling\n";
        }

        if (towerStat == true) itemText.text += "\n";

        if (bodyObject.SlowPercentage > 0) itemText.text += "<color=green>+" + bodyObject.SlowPercentage + "%</color> Slow\n";
        if (bodyObject.SlowDuration > 0) itemText.text += "<color=green>+" + bodyObject.SlowDuration + "</color> Slow Duration\n";

        if (bodyObject.LifeSteal == true) itemText.text += "Lifesteal on kill (<color=green>" + Player.Instance.GetLifeStealChance() + "%</color> chance)\n";

        if (bodyObject.ChanceForDoubleDamage == true) itemText.text += "Double Damage (<color=green>" + Player.Instance.GetDoubleDamageChance() + "%</color> chance)\n";

        if (bodyObject.AdditionalProjectiles == 1) itemText.text += "<color=green>+" + bodyObject.AdditionalProjectiles + "</color> Projectile";
        else if (bodyObject.AdditionalProjectiles > 1) itemText.text += "<color=green>+" + bodyObject.AdditionalProjectiles + "</color> Projectiles";
    }
}
