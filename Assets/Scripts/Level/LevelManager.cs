using System;
using System.Collections;
using Marvin.PoolingSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour, IPoolingList
{
    public static LevelManager Instance;
    public Transform[] enemyWayPoints;

    [Space]
    [SerializeField] public LevelObj[] levels;

    private GameObject startLevelButton;

    [HideInInspector] public int currentLevel { get; private set; }

    [Space]
    [SerializeField] private int enemiesAlive;
    [SerializeField] private int activeSpawners;
    [SerializeField] private int levelToDisplay;

    //[Space]
    //[SerializeField] private GameObject endingScreen;

    public PoolingSystem.PoolObjectInfo poolingList { get; set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {                                                                                                   
            Destroy(gameObject);
        }
        levelToDisplay = currentLevel + 1;
    }
    private void Start()
    {
        startLevelButton = IngameController.Instance.playerUI.startNextLevelButton;

        startLevelButton.SetActive(true);
    }
    private void OnEnable()
    {
        Enemy.enemyHasDied += CheckForNextRound;
    }
    private void OnDisable()
    {
        Enemy.enemyHasDied -= CheckForNextRound;
    }
    public Vector3 GetWayPoint(int number)
    {
        return enemyWayPoints[number].position;
    }
    public void StartNextLevel()
    {
        if (startLevelButton != null) startLevelButton.SetActive(false);

        if (currentLevel < levels.Length)
        {
            for (int i = 0; i < levels[currentLevel].levelEnemyValues.Length; i++)
            {
                activeSpawners++;
                StartCoroutine(StartEnemySpawn(levels[currentLevel].levelEnemyValues[i], levels[currentLevel].levelEnemyValues[i].waveStartDelay));
            }
        }
    }
    private IEnumerator StartEnemySpawn(LevelEnemyValues enemyObj, float waveStartDelay)
    {
        yield return new WaitForSeconds(waveStartDelay);
        int amount = 0;

        while (amount < enemyObj.amount)
        {
            enemiesAlive++;
            Enemy enemy = PoolingSystem.SpawnObject(enemyObj.enemy, enemyWayPoints[0].position, Quaternion.identity, PoolingSystem.PoolingParentGameObject.Enemy).GetComponent<Enemy>();
            enemy.SetMaxHealth(enemyObj.healthScaling);
            amount++;

            if (amount >= enemyObj.amount) activeSpawners--;

            yield return new WaitForSeconds(enemyObj.spawnRate);
        }
    }
    private void CheckForNextRound()
    {
        enemiesAlive--;
        if (enemiesAlive == 0 && activeSpawners == 0)
        {
            currentLevel++;
            levelToDisplay = currentLevel + 1;
            if (currentLevel < levels.Length)
            {
                startLevelButton.SetActive(true);
            }
            else
            {
                EndGame();
            }
        }
    }
    private void EndGame()
    {
        //PlayerUI.Instance.ActivateGameOverScreen();
    }
}
