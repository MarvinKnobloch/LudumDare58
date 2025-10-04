using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelObj", menuName = "ScriptableObject/Levels")]
public class LevelObj : ScriptableObject
{
    public LevelEnemyValues[] levelEnemyValues;
    [TextArea] public string description;
}

[Serializable]
public struct LevelEnemyValues
{
    public GameObject enemy;
    public int amount;
    public float spawnRate;
    public float healthScaling;
}