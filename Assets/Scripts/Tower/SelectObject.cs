using Marvin.AudioSystem;
using Marvin.PoolingSystem;
using Tower;
using UnityEngine;

public class SelectObject : MonoBehaviour
{
    private Controls controls;
    [SerializeField] private RangeIndicator rangeIndicator;

    [SerializeField] private Tutorial tutorial; //desi

    [SerializeField] private AudioObj pickUpItemSound;

    private void Awake()
    {
        controls = new Controls();
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    private void Update()
    {
        if (controls.Player.Confirm.WasPerformedThisFrame())
        {
            if (IngameController.Instance.menuController.gameIsPaused) return;
            if (GameManager.Instance.showIntro == true) return;

            Collider2D[] cols = Physics2D.OverlapCircleAll(Utility.MousePostion(), 0.01f);

            if(cols.Length > 0)
            {
                for (int i = 0; i < cols.Length; i++)
                {
                    if (cols[i].TryGetComponent(out WorldItem worldItem))
                    {

                        IngameController.Instance.playerUI.inventory.AddResource(worldItem.itemInformationen, worldItem.dropAmount);
                        if (worldItem.testItem == false) worldItem.ReturnToItemPool();

                        AudioManager.Instance.PlayAudioObjOneShot(pickUpItemSound);

                        //desi
                        if (GameManager.Instance.showTutorial == true)
                        {
                            tutorial.TryAdvanceHint(2);
                        }
                        break;
                    }
                    else if (cols[i].TryGetComponent(out TowerBase towerBase))
                    {
                        //desi
                        if (GameManager.Instance.showTutorial == true)
                        {
                            tutorial.TryAdvanceHint(3);
                        }

                        IngameController.Instance.playerUI.inventory.SetCurrentTower(towerBase);
                        break;
                    }
                   
                }
            }
        }
    }
}
