using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyClickHandler : MonoBehaviour
{
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }
    //Soll nicht für jeden Enemy gemacht werden sondern erkennt clicks über kamera
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Enemy enemyScript = hit.collider.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    Debug.Log("Enemy wurde angeklickt!");
                    enemyScript.TakeDamage(enemyScript.MaxValue);

                }
            }
        }
    }
}
