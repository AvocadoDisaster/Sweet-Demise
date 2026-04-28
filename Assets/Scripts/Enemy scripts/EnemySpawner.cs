using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EnemySpawner : MonoBehaviour
{
    
    public static EnemySpawner Instance { get; private set; }

   
    [System.Serializable]
    public class EnemyEntry
    {
        public GameObject prefab;
        
        [Range(1, 100)]
        public int weight = 10;
    }

    [Header("Enemy Pool")]
  
    public List<EnemyEntry> enemyPool = new List<EnemyEntry>();

    [Header("Spawn Settings")]
    
    public string spawnPointTag = "SpawnPoint";
   
    public float baseSpawnInterval = 3f;
   
    public int   baseMaxEnemies    = 6;
    
    public bool  autoStart         = true;

    [Header("Difficulty Scaling")]
    
    public float difficultyTickInterval = 30f;
   
    public float intervalReduction = 0.2f;
    
    public int   maxEnemiesPerLevel = 2;
    
    public int   maxDifficultyLevel = 10;

    [Header("Scenes to Ignore")]
    
    public List<string> disabledScenes = new List<string>();

  
    public int  DifficultyLevel   { get; private set; }
    public int  ActiveEnemyCount  => trackedEnemies.Count;
    public bool IsSpawning        { get; private set; }

    
    private List<GameObject> trackedEnemies = new List<GameObject>();
    private List<Transform>  spawnPoints    = new List<Transform>();
    private float spawnTimer;
    private float difficultyTimer;
    private float currentInterval;
    private int   currentMaxEnemies;

   
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentInterval   = baseSpawnInterval;
        currentMaxEnemies = baseMaxEnemies;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
       
        trackedEnemies.RemoveAll(e => e == null);

        RefreshSpawnPoints();

        if (autoStart && !disabledScenes.Contains(scene.name))
            StartSpawning();
        else
            StopSpawning();
    }

    private void Update()
    {
        if (!IsSpawning) return;

        
        difficultyTimer += Time.deltaTime;
        if (difficultyTimer >= difficultyTickInterval)
        {
            difficultyTimer = 0f;
            LevelUpDifficulty();
        }

       
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentInterval)
        {
            spawnTimer = 0f;
            TrySpawn();
        }

       
        trackedEnemies.RemoveAll(e => e == null);
    }

    
    public void StartSpawning()
    {
        RefreshSpawnPoints();
        IsSpawning = true;
        spawnTimer = 0f;
    }

    public void StopSpawning()
    {
        IsSpawning = false;
    }

    public void DestroyAllActiveEnemies()
    {
        foreach (GameObject e in trackedEnemies)
            if (e != null) Destroy(e);
        trackedEnemies.Clear();
    }

    public void ResetDifficulty()
    {
        DifficultyLevel   = 0;
        currentInterval   = baseSpawnInterval;
        currentMaxEnemies = baseMaxEnemies;
        difficultyTimer   = 0f;
    }

    
    private void TrySpawn()
    {
        if (trackedEnemies.Count >= currentMaxEnemies) return;
        if (spawnPoints.Count == 0) return;
        if (enemyPool.Count == 0) return;

        Transform point  = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject prefab = PickWeightedRandom();

        if (prefab == null) return;

        GameObject enemy = Instantiate(prefab, point.position, Quaternion.identity);
        trackedEnemies.Add(enemy);
    }

    private void RefreshSpawnPoints()
    {
        spawnPoints.Clear();
        GameObject[] pts = GameObject.FindGameObjectsWithTag(spawnPointTag);
        foreach (GameObject pt in pts)
            spawnPoints.Add(pt.transform);

        
    }

    private GameObject PickWeightedRandom()
    {
        int totalWeight = 0;
        foreach (EnemyEntry e in enemyPool)
            totalWeight += e.weight;

        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;
        foreach (EnemyEntry e in enemyPool)
        {
            cumulative += e.weight;
            if (roll < cumulative) return e.prefab;
        }
        return enemyPool[0].prefab;
    }

    private void LevelUpDifficulty()
    {
        if (DifficultyLevel >= maxDifficultyLevel) return;

        DifficultyLevel++;
        currentInterval   = Mathf.Max(0.5f, baseSpawnInterval - (intervalReduction * DifficultyLevel));
        currentMaxEnemies = baseMaxEnemies + (maxEnemiesPerLevel * DifficultyLevel);

        Debug.Log($"[EnemySpawner] Difficulty → Level {DifficultyLevel} | Interval: {currentInterval:F1}s | Max: {currentMaxEnemies}");
    }
}
