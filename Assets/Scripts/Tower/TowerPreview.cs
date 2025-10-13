using System;
using Tower;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPreview : MonoBehaviour
{
    [SerializeField] private GameObject towerToBuild;
    [SerializeField] private RangeIndicator rangeIndicator;
    [SerializeField] private TowerValues defaultTower;

    [Space]
    [SerializeField] private LayerMask buildLayer;
    [SerializeField] private LayerMask buildArea;
    [SerializeField] private Color canBuildColor;
    [SerializeField] private Color cantBuildColor;
    private bool canBuild;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;

    public static event Action buildCanceled;

    private Controls controls;

    private void Awake()
    {
        controls = new Controls();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        BuildCheck();
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        BuildCheck();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        BuildCheck();
    }
    private void Update()
    {
        transform.root.position = Utility.MousePostion();
        rangeIndicator.gameObject.transform.position = Utility.MousePostion();
        if (controls.Player.Confirm.WasPerformedThisFrame())
        {
            BuildCheck();
            if (canBuild == true && Player.Instance.CheckForTowerCosts() == true)
            {
                Instantiate(towerToBuild,transform.root.position, Quaternion.identity);
                Player.Instance.BuyTower();

                //after BuildingTower
                if (Player.Instance.CheckForTowerCosts() == false) 
                {
                    CancelBuildMode();
                }
                BuildCheck();
            }
        }
        if (controls.Player.Cancel.WasPerformedThisFrame() || controls.Menu.MenuEsc.WasPerformedThisFrame())
        {
            CancelBuildMode();
        }
    }
    private void BuildCheck()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius * transform.localScale.x, buildLayer);

        if (colls.Length > 0)
        {
            canBuild = false;
            spriteRenderer.color = cantBuildColor;
        }
        else
        {
            Collider2D[] area = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius * transform.localScale.x, buildArea);
            if(area.Length > 0)
            {
                canBuild = true;
                spriteRenderer.color = canBuildColor;
            }
            else
            {
                canBuild = false;
                spriteRenderer.color = cantBuildColor;
            }
        }
    }
    private void CancelBuildMode()
    {
        rangeIndicator.gameObject.SetActive(false);
        buildCanceled?.Invoke();
        transform.root.gameObject.SetActive(false);
    }
    public void SetRangeIndicator()
    {
        if (rangeIndicator == null) rangeIndicator = IngameController.Instance.playerUI.inventory.rangeIndicator;

        rangeIndicator.gameObject.transform.position = Utility.MousePostion();
        rangeIndicator.gameObject.SetActive(true);
        rangeIndicator.DrawCircle(defaultTower.rangeScalingPercantage * (defaultTower.startBonusRange * 0.01f));
    }
}
