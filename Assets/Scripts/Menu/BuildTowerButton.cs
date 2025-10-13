using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildTowerButton : MonoBehaviour
{
    private Controls controls;

    [SerializeField] private Image buttonImage;
    [SerializeField] private GameObject defaultTowerPreview;
    [SerializeField] private GameObject towerUIPreview;
    [SerializeField] private Sprite buttonReleasedSprite;
    [SerializeField] private Sprite buttonPressedSprite;
    [SerializeField] private GameObject greenLight;
    [SerializeField] private GameObject redLight;
    private GameObject towerPreview;
    private bool towerPreviewActive;

    private void Awake()
    {
        controls = new Controls();
    }
    private void OnEnable()
    {
        controls.Enable();
        controls.Player.BuildButton.performed += BuildButtonHotkey;

        TowerPreview.buildCanceled += ButtonReleased;
        Player.soulsChanged += CheckLights;
    }
    private void OnDisable()
    {
        controls.Player.BuildButton.performed -= BuildButtonHotkey;

        TowerPreview.buildCanceled -= ButtonReleased;
        Player.soulsChanged -= CheckLights;

        controls.Disable();
    }
    private void Start()
    {
        towerPreview = Instantiate(defaultTowerPreview, transform.position, Quaternion.identity);
        towerPreview.SetActive(false);
        //TextMeshProUGUI buttonText = GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>();
        //buttonText.text = "Build (" + Player.Instance.GetTowerCosts().ToString() +")";
    }
    public void SelectTowerButton()
    {
        if(towerPreview.activeSelf == false && Player.Instance.CheckForTowerCosts() == true)
        {
            towerPreviewActive = true;
            buttonImage.sprite = buttonPressedSprite;
            towerPreview.transform.position = Utility.MousePostion();
            towerPreview.SetActive(true);
            towerUIPreview.SetActive(true);
        }
    }
    private void ButtonReleased()
    {
        towerUIPreview.SetActive(false);
        buttonImage.sprite = buttonReleasedSprite;
        towerPreviewActive = false;
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

    private void BuildButtonHotkey(InputAction.CallbackContext context)
    {
        if (IngameController.Instance.menuController.gameIsPaused) return;

        SelectTowerButton();
    }
    public bool TowerPreviewActive()
    {
        return towerPreviewActive;
    }
}
