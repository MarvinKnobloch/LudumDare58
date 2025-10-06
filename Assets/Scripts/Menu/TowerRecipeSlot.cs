using TMPro;
using Tower;
using UnityEngine;
using UnityEngine.UI;

public class TowerRecipeSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI towerName;
    [SerializeField] private Image towerIcon;
    [SerializeField] private GameObject[] slots;

    public void SetSlot(TowerRecipe towerRecipe)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetActive(false);
        }

        towerName.text = "<u>" + towerRecipe.towerName + "</u>";
        towerIcon.sprite = towerRecipe.towerIcon;

        for (int i = 0;i < towerRecipe.Recipe.Count; i++)
        {
            slots[i].SetActive(true);
            slots[i].transform.GetChild(0).GetComponentInChildren<Image>().sprite = towerRecipe.Recipe[i].Sprite;
        }
    }
}
