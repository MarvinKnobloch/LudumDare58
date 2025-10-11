using Tower;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerValues", menuName = "ScriptableObject/TowerValues")]
public class TowerValues : ScriptableObject
{
    public int damageScalingPercantage = 100;
    public int startBonusAttack;
    public float baseAttackSpeed = 1;
    public float rangeScalingPercantage = 100;
    public float startBonusRange;
    public float aoeRadius = 0;
    public GameObject projectilePrefab;
    public TargetType targetType;
    [Space]
    public int slowPercentage;
    public float slowDuration;
    [Space]
    public int additionalProjectiles;
    [Space]
    public GameObject objectToSpawn;
    [Space]
    public bool chanceForDoubleDamage;
}