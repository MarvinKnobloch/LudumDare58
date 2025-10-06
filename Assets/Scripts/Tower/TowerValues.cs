using UnityEngine;

[CreateAssetMenu(fileName = "TowerValues", menuName = "ScriptableObject/TowerValues")]
public class TowerValues : ScriptableObject
{
    public int baseDamage = 1;
    public float baseAttackSpeed = 1;
    public float baseAttackRange = 3;
}