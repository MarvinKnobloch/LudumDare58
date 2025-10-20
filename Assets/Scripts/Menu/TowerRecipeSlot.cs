using TMPro;
using Tower;
using UnityEngine;
using UnityEngine.UI;

public class TowerRecipeSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI towerName;
    [SerializeField] private Image towerIcon;
    [SerializeField] private GameObject[] slots;
    [SerializeField] private Image[] slotChilds;
    [SerializeField] private Sprite[] slotImages;

    public TowerRecipe towerRecipe;

    public void SetSlot(TowerRecipe _towerRecipe)
    {
        towerRecipe = _towerRecipe;
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetActive(false);
        }

        towerName.text = "<u>" + towerRecipe.towerName + "</u>";
        towerIcon.sprite = towerRecipe.towerIcon;

        UpdateSlots();
    }
    public void UpdateSlots()
    {
        int towerUnlocked = PlayerPrefs.GetInt(towerRecipe.towerName);

        for (int i = 0; i < towerRecipe.Recipe.Count; i++)
        {
            if (towerUnlocked == 1) ShowSlot(towerRecipe, i);
            else
            {
                if (towerRecipe.partUnlocked[i] == true)
                {
                    ShowSlot(towerRecipe, i);
                }
                else
                {
                    slots[i].SetActive(true);
                    slotChilds[i].gameObject.SetActive(true);
                    switch (towerRecipe.Recipe[i].Part)
                    {
                        case BodyPart.Accessory:
                            slotChilds[i].sprite = slotImages[0];
                            break;
                        case BodyPart.Head:
                            slotChilds[i].sprite = slotImages[1];
                            break;
                        case BodyPart.Arm:
                            slotChilds[i].sprite = slotImages[2];
                            break;
                        case BodyPart.Torso:
                            slotChilds[i].sprite = slotImages[3];
                            break;
                        case BodyPart.Weapon:
                            slotChilds[i].sprite = slotImages[4];
                            break;
                    }
                }
            }
        }
    }

    private void ShowSlot(TowerRecipe towerRecipe, int i)
    {
        slots[i].SetActive(true);
        slots[i].transform.GetChild(0).GetComponentInChildren<Image>().sprite = towerRecipe.Recipe[i].Sprite;
        slots[i].transform.GetChild(1).gameObject.SetActive(false);
    }
}
