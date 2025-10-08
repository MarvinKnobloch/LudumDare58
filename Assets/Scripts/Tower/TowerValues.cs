using Tower;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerValues", menuName = "ScriptableObject/TowerValues")]
public class TowerValues : ScriptableObject
{
    public int baseDamage = 1;
    public float baseAttackSpeed = 1;
    public float baseAttackRange = 3;
    public float aoeRadius = 0;
    public GameObject projectilePrefab;
    public WeaponType weaponType;
    public TargetType targetType;
    public bool slow;
    public int slowPercentage;
    public int slowDuration;
}