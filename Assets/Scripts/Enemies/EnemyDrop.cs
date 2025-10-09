using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private DropValues[] dropValues;
    [SerializeField] private float scatterRadius = 0.15f; 
    [SerializeField] private bool addForce = true;

    [SerializeField] private Collider2D buildArea;

    public void DropItems(Vector3 position)
    {
        for (int i = 0; i < dropValues.Length; i++)
        {
            var drop = dropValues[i];
            if (drop.item == null || Random.value > drop.dropChance)
                continue;

            for (int j = 0; j < drop.amount; j++)
            {
                Vector2 randomDirection = Random.insideUnitCircle * scatterRadius;
                Vector3 dropPosition = position + new Vector3(randomDirection.x, randomDirection.y, 0f);

                if (DropAreaConfiner.Instance != null)
                    dropPosition = DropAreaConfiner.Instance.ClampToArea(dropPosition);


                GameObject newDrop = Instantiate(drop.item, dropPosition, Quaternion.identity);

                if (addForce)
                {
                    Rigidbody2D rb = newDrop.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Vector2 randomForce = Random.insideUnitCircle * 0.02f;
                        rb.AddForce(randomForce, ForceMode2D.Force); 

                        float randomTorque = Random.Range(-0.05f, 0.05f);
                        rb.AddTorque(randomTorque, ForceMode2D.Force);

                        rb.linearDamping = 10f;
                        rb.angularDamping = 10f;
                        rb.gravityScale = 0f;
                        rb.mass = 1f; 
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class DropValues
    {
        public GameObject item;
        [Range(0f, 1f)] public float dropChance = 1f;
        public int amount = 1;
    }
}
