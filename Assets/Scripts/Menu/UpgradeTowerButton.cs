using UnityEngine;
using UnityEngine.UI;

public class UpgradeTowerButton : MonoBehaviour
{
    [SerializeField] private GameObject upgradeClosed;
    [SerializeField] private GameObject upgradeOpen;

    [SerializeField] private Image upgradeProgressionBar;


    public void UpgradeTower()
    {
        IngameController.Instance.playerUI.inventory.currentSelectedTower.UpgradeTower();
    }
    public void UpdateUpgradeTowerInfo(bool toggle, int percantage)
    {
        if(toggle == true)
        {
            upgradeOpen.SetActive(true);
            upgradeClosed.SetActive(false);
        }
        else
        {
            upgradeOpen.SetActive(false);
            upgradeClosed.SetActive(true);

        }
        upgradeProgressionBar.fillAmount = (float)percantage / 100;
    }
}
