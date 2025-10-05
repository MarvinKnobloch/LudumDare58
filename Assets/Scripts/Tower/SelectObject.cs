using Tower;
using UnityEngine;

public class SelectObject : MonoBehaviour
{
    private Controls controls;
    [SerializeField] private RangeIndicator rangeIndicator;
    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    private void Update()
    {
        if (controls.Player.Confirm.WasPerformedThisFrame())
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(Utility.MousePostion(), 0.01f);

            if(cols.Length > 0)
            {
                for (int i = 0; i < cols.Length; i++)
                {
                    if (cols[i].TryGetComponent(out WorldItem worldItem))
                    {
                        IngameController.Instance.playerUI.inventory.AddResource(worldItem.itemInformationen, worldItem.dropAmount);
                    }
                    else if (cols[i].TryGetComponent(out TowerBase towerBase))
                    {
                        rangeIndicator.gameObject.transform.position = towerBase.gameObject.transform.position;
                        rangeIndicator.gameObject.SetActive(true);
                        rangeIndicator.DrawCircle(3);
                        IngameController.Instance.playerUI.inventory.SetCurrentTower(towerBase);
                        break;
                    }

                }
            }
        }
    }
}
