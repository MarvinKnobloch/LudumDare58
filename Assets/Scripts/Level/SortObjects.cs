using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SortObjects : MonoBehaviour
{
    [SerializeField] private float sortInterval;
    private float timer;
    private int maxSprites = 199;

    public static List<SpriteRenderer> activeEnemiesSprites = new List<SpriteRenderer>();
    private void Awake()
    {
        activeEnemiesSprites.Clear();
    }

    private void Start()
    {
        enabled = false;
    }
    void Update()
    {
        //könnte man enable/disable, wenn lvl start für bessere Performence.
        timer += Time.deltaTime;
        if (timer > sortInterval)
        {
            timer = 0;
            Sort();
        }
    }
    private void Sort()
    {
        if (activeEnemiesSprites.Count <= 1) return;

        activeEnemiesSprites = activeEnemiesSprites.OrderBy(tr => tr.transform.position.y).ToList();

        for (int i = 0; i < activeEnemiesSprites.Count; i++)
        {
            if(i < maxSprites)
            {
                activeEnemiesSprites[i].sortingOrder = -i;
            }
            else
            {
                activeEnemiesSprites[i].sortingOrder = -maxSprites;
            }
        }
    }
}
