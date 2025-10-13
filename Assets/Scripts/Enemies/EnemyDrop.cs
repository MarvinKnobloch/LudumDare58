using System.Collections;
using Marvin.PoolingSystem;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private DropValues[] dropValues;
    [SerializeField] private float scatterRadius = 0.15f;
    [SerializeField] private bool useAnimation = true;

    [Header("Animation Settings")]
    [SerializeField] private float popDuration = 0.4f;
    [SerializeField] private float heightY = 0.5f;
    [SerializeField] private AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

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

                GameObject newDrop = PoolingSystem.SpawnObject(drop.item, position, Quaternion.identity, PoolingSystem.PoolingParentGameObject.Item);
                
                if (useAnimation)
                {
                    LootPop pop = newDrop.GetComponent<LootPop>();
                    if (pop == null)
                        pop = newDrop.AddComponent<LootPop>();

                    pop.StartPop(dropPosition, popDuration, heightY, animCurve);
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

public class LootPop : MonoBehaviour
{
    private void OnEnable()
    {
        StopAllCoroutines();
    }
    private void OnDisable()
    {
        Debug.Log("itemDisable");
        StopAllCoroutines();
    }
    public void StartPop(Vector2 endPosition, float popDuration, float heightY, AnimationCurve animCurve)
    {
        StartCoroutine(AnimCurveSpawnRoutine(endPosition, popDuration, heightY, animCurve));
    }

    private IEnumerator AnimCurveSpawnRoutine(Vector2 endPoint, float popDuration, float heightY, AnimationCurve animCurve)
    {
        Vector2 startPoint = transform.position;
        float timePassed = 0f;
        float heightVariation = Random.Range(heightY * 0.8f, heightY * 1.2f);

        while (timePassed < popDuration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / popDuration;
            float heightT = animCurve.Evaluate(linearT);
            float height = heightY * animCurve.Evaluate(linearT);

            transform.position =
                Vector2.Lerp(startPoint, endPoint, linearT)
                + new Vector2(0f, height);

            yield return null;
        }

        transform.position = endPoint;
    }
}
