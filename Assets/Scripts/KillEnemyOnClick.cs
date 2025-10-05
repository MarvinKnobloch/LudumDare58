using Tower;
using UnityEngine;

public class KillEnemyOnClick : MonoBehaviour
{
    private Controls controls;
    [SerializeField] private int damage = 100;
    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    private void Update()
    {
        if (controls.Player.Confirm.WasPerformedThisFrame())
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(Utility.MousePostion(), 0.01f);

            if (cols.Length > 0)
            {
                for (int i = 0; i < cols.Length; i++)
                {
                    if (cols[i].TryGetComponent(out Enemy enemy))
                    {
                        enemy.TakeDamage(damage);
                        break;
                    }
                }
            }
        }
    }
}
