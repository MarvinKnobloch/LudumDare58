using Tower;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerValues", menuName = "ScriptableObject/TowerValues")]
public class TowerValues : ScriptableObject
{
    public int damageScaling = 100;
    public int baseDamage = 1;
    public float baseAttackSpeed = 1;
    public float baseAttackRange = 3;
    public float aoeRadius = 0;
    public GameObject projectilePrefab;
    public TargetType targetType;
    [Space]
    public int slowPercentage;
    public int slowDuration;
    [Space]
    public int additionalProjectiles;
    [Space]
    public GameObject objectToSpawn;
}