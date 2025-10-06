using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    [SerializeField] private GameObject DefaultTowerPreview;
    private GameObject towerPreview;
    

    private void Awake()
    {
        towerPreview = Instantiate(DefaultTowerPreview, transform.position, Quaternion.identity);
        towerPreview.SetActive(false);
        TextMeshProUGUI buttonText = GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = "Build (" + Player.Instance.GetTowerCosts().ToString() +")";
    }
    public void SelectTowerButton()
    {
        if(towerPreview.activeSelf == false && Player.Instance.CheckForTowerCosts() == true)
        {
            towerPreview.transform.position = Utility.MousePostion();
            towerPreview.SetActive(true);
        }
    }
}
