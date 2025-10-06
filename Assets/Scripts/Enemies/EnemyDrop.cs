using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private DropValues[] dropValues;
    [SerializeField] private float scatterRadius = 8f;  
    [SerializeField] private bool addForce = true;

    public void DropItems(Vector3 position)
    {
        for (int i = 0; i < dropValues.Length; i++)
        {
            if (dropValues[i].item == null)
                continue;

            int amountToDrop = (i < dropValues.Length) ? dropValues[i].amount : 1;

            for (int j = 0; j < amountToDrop; j++)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(3f, scatterRadius);
                Vector3 dropPosition = position + new Vector3(randomDirection.x, randomDirection.y, 0f);

                GameObject drop = Instantiate(dropValues[i].item, dropPosition, Quaternion.identity);

                if (addForce)
                {
                    Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Vector2 randomForce = randomDirection * Random.Range(8f, 16f);
                        rb.AddForce(randomForce, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }
}


[System.Serializable]
public class DropValues
{
    public GameObject item;   
    
    public float dropChance;  

    public int amount;        
}

