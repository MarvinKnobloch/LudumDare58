using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private DropValues[] dropValues;
    [SerializeField] private float scatterRadius = 8f;
    [SerializeField] private bool addForce = true;

    [SerializeField] private Collider2D buildArea; 


    public void DropItems(Vector3 position)
    {
        for (int i = 0; i < dropValues.Length; i++)
        {
            var drop = dropValues[i];

            if (drop.item == null)
                continue;

            if (Random.value > drop.dropChance)
                continue;

            int amountToDrop = drop.amount;

            for (int j = 0; j < amountToDrop; j++)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(3f, scatterRadius);
                Vector3 dropPosition = position + new Vector3(randomDirection.x, randomDirection.y, 0f);

                GameObject newDrop = Instantiate(drop.item, dropPosition, Quaternion.identity);

                if (addForce)
                {
                    Rigidbody2D rb = newDrop.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Vector2 randomForce = Random.insideUnitCircle.normalized * Random.Range(3f, 6f);
                        rb.AddForce(randomForce, ForceMode2D.Impulse);

                        float randomTorque = Random.Range(-6, 6f); //Drehung
                        rb.AddTorque(randomTorque, ForceMode2D.Impulse);

                        rb.linearDamping = Random.Range(1f, 3f);   //Luftwiderstand
                        rb.angularDamping = Random.Range(1f, 2f); //Rotationswiderstand
                        

                        
                    }
                }
            }
        }
    }

    public Vector3 GetRandomPositionInsideBuildArea()
    {


        Collider2D buildArea = BuildAreaManager.Instance.AreaCollider;

        Bounds bounds = buildArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector3(x, y, 0f);
    }
}

[System.Serializable]
public class DropValues
{
    public GameObject item;
    [Range(0f, 1f)] public float dropChance = 0.75f; 
    public int amount = 1;
}
