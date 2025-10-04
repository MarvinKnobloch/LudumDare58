using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPreview : MonoBehaviour
{
    [SerializeField] private LayerMask buildLayer;
    [SerializeField] private LayerMask buildArea;
    [SerializeField] private Color canBuildColor;
    [SerializeField] private Color cantBuildColor;
    private bool isBuild;
    private bool canBuild;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;

    private Controls controls;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        BuildCheck();
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
        if (controls.Player.Confirm.WasPerformedThisFrame())
        {
            BuildCheck();
            if (canBuild == true)
            {
                Debug.Log("Build");
            }
        }
        if (controls.Player.Cancel.WasPerformedThisFrame())
        {
            transform.root.gameObject.SetActive(false);
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
}
