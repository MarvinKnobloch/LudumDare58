using UnityEngine;

public class TowerButton : MonoBehaviour
{
    [SerializeField] private GameObject DefaultTowerPreview;
    private GameObject towerPreview;

    private void Awake()
    {
        towerPreview = Instantiate(DefaultTowerPreview, transform.position, Quaternion.identity);
        towerPreview.SetActive(false);
    }
    public void SelectTowerButton()
    {
        if(towerPreview.activeSelf == false)
        {
            towerPreview.transform.position = Utility.MousePostion();
            towerPreview.SetActive(true);
        }
    }
}
