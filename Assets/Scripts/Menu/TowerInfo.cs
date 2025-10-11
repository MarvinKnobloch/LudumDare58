using TMPro;
using UnityEngine;

public class TowerInfo : MonoBehaviour
{
    [SerializeField] private GameObject scrollClosed;
    [SerializeField] private GameObject scrollOpened;

    [SerializeField] private TextMeshProUGUI upgradeText;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private TextMeshProUGUI specialText;

    public void TowerInfoUpdate()
    {
        
    }
    public void ToggleScrolls()
    {
        if (scrollClosed.activeSelf)
        {
            scrollClosed.SetActive(false);
            scrollOpened.SetActive(true);
        }
        else
        {
            scrollClosed.SetActive(true);
            scrollOpened.SetActive(false);
        }
    }
}
