using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildTowerButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private GameObject DefaultTowerPreview;
    [SerializeField] private Sprite buttonReleasedSprite;
    [SerializeField] private Sprite buttonPressedSprite;
    [SerializeField] private GameObject greenLight;
    [SerializeField] private GameObject redLight;
    private GameObject towerPreview;


    private void OnEnable()
    {
        TowerPreview.buildCanceled += ButtonReleased;
        Player.soulsChanged += CheckLights;
    }
    private void OnDisable()
    {
        TowerPreview.buildCanceled -= ButtonReleased;
        Player.soulsChanged -= CheckLights;
    }
    private void Start()
    {
        towerPreview = Instantiate(DefaultTowerPreview, transform.position, Quaternion.identity);
        towerPreview.SetActive(false);
        //TextMeshProUGUI buttonText = GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>();
        //buttonText.text = "Build (" + Player.Instance.GetTowerCosts().ToString() +")";
    }
    public void SelectTowerButton()
    {
        if(towerPreview.activeSelf == false && Player.Instance.CheckForTowerCosts() == true)
        {
            buttonImage.sprite = buttonPressedSprite;
            towerPreview.transform.position = Utility.MousePostion();
            towerPreview.SetActive(true);
        }
    }
    private void ButtonReleased()
    {
        buttonImage.sprite = buttonReleasedSprite;
    }
    private void CheckLights(int amount)
    {
        if (amount < Player.Instance.GetTowerCosts())
        {
            if (redLight.activeSelf) return;

            redLight.SetActive(true);
            greenLight.SetActive(false);
        }
        else
        {
            if (greenLight.activeSelf) return;

            greenLight.SetActive(true);
            redLight.SetActive(false);
        }
    }
}
