using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private int[] dropAmounts;

    public void DropItems(Vector3 position)
    {
        // Gehe durch alle möglichen Items in der Liste
        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            if (itemPrefabs[i] == null)
            {
                continue;
            }

            int amountToDrop;

            if (i < dropAmounts.Length)
            {
                amountToDrop = dropAmounts[i];
            }
            else
            {
                amountToDrop = 1;
            }

            for (int j = 0; j < amountToDrop; j++)
            {
                GameObject item = Instantiate(itemPrefabs[i], position, Quaternion.identity);

                // Sicherstellen, dass das Item vorne (Z= -1 oder -2) sichtbar ist
                item.transform.position = new Vector3(position.x, position.y, -2f);
            }
        }
    }
}
