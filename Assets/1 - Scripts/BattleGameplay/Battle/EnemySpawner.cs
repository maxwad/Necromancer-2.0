using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BossData {
    public bool wasCreated = false;
    public bool isDead = false;
    public float bound = 0;
}

public class EnemySpawner : MonoBehaviour
{
    private ArmyStrength currentStrength;
    private List<EnemiesTypes> enemiesList = new List<EnemiesTypes>();
    public List<int> enemiesQuantityList = new List<int>();
    private int currentCommonQuantity = 0;
    private int maxQuantity = 0;
    private int removeQuantity = 0;
    public List<GameObject> enemiesOnTheMap = new List<GameObject>();

    [Space]
    [SerializeField] private List<GameObject> spawnPositions;

    [Space]
    private bool canISpawn = false;
    private int enemySlowCount = 100;
    private int enemyTooSlowCount = 150;
    private float spawnOffset = 5f;

    [Space]
    private Coroutine spawnCoroutine;
    //private float waitNextEnemyTimeFast = 0.1f;
    private float waitNextEnemyTimeFast = .1f;
    private float waitNextEnemyTimeSlow = 0.5f;
    private float waitNextEnemyTimeStop = 2f;
    private WaitForSeconds waitNextEnemyFast;
    private WaitForSeconds waitNextEnemySlow;
    private WaitForSeconds waitNextEnemyStop;
    private WaitForSeconds waitNextEnemy;

    //private BattleMap battleMap;

    private bool[,] battleMap;
    private EnemyEffector enemyEffector;
    private BattleUIManager battleUIManager;
    private BossData[] bossBounds;

    private void Start()
    {
        enemyEffector = GetComponent<EnemyEffector>();
        battleUIManager = GlobalStorage.instance.battleIUManager;
    }

    public void Initialize(Army army)
    {
        currentStrength = army.strength;

        enemiesList.Clear();
        foreach(var squad in army.squadList)
            enemiesList.Add(squad);

        enemiesQuantityList.Clear();
        foreach(var squad in army.quantityList)
            enemiesQuantityList.Add(squad);

        waitNextEnemyFast = new WaitForSeconds(waitNextEnemyTimeFast);
        waitNextEnemySlow = new WaitForSeconds(waitNextEnemyTimeSlow);
        waitNextEnemyStop = new WaitForSeconds(waitNextEnemyTimeStop);
        waitNextEnemy = waitNextEnemyFast;

        currentCommonQuantity = 0;

        for(int i = 0; i < enemiesQuantityList.Count; i++)
            currentCommonQuantity += enemiesQuantityList[i];

        maxQuantity = currentCommonQuantity;

        CreateBossStructure();

        battleUIManager.enemyPart.SetStartEnemiesParameters(currentCommonQuantity, bossBounds);
    }

    private void CreateBossStructure()
    {
        float enemyQuantity = maxQuantity;
        float bossCount = (int)currentStrength;
        float bossPortion = Mathf.Round((maxQuantity / (bossCount + 1)));
        bossBounds = new BossData[(int)bossCount];

        for(int i = 0; i < bossCount; i++)
        {
            bossBounds[i] = new BossData();
            enemyQuantity -= bossPortion;
            bossBounds[i].bound = enemyQuantity;
            bossBounds[i].isDead = false;
            bossBounds[i].wasCreated = false;
        }
    }

    private void Update()
    {
        if (enemiesOnTheMap.Count < enemySlowCount) waitNextEnemy = waitNextEnemyFast;

        if (enemiesOnTheMap.Count > enemySlowCount) waitNextEnemy = waitNextEnemySlow;

        if (enemiesOnTheMap.Count > enemyTooSlowCount) waitNextEnemy = waitNextEnemyStop;
    }

    public void ReadyToSpawnEnemy()
    {
        canISpawn = true;
        removeQuantity = 0;
        battleMap = GetComponent<BattleMap>().battleArray;
        spawnCoroutine = StartCoroutine(SpawnEnemy());
        enemyEffector.Init(currentStrength);
    }

    public void StopSpawnEnemy()
    {
        canISpawn = false;
        enemyEffector.StopEffector();
        enemiesOnTheMap.Clear();
    }

    private IEnumerator SpawnEnemy()
    {
        //List<float> currentProbably = new List<float>();

        while (canISpawn == true)
        {      
            if (currentCommonQuantity == 0)
            {
                canISpawn = false;
                if(spawnCoroutine != null) StopCoroutine(spawnCoroutine);
                
                break;
            }

            Vector3 randomPosition = GetSpawnPosition();

            if (randomPosition != Vector3.zero)
            {
                int randomIndex = 0;
                bool finded = false;

                while (finded == false && currentCommonQuantity != 0)
                {
                    randomIndex = Random.Range(0, enemiesList.Count);
                    if (enemiesQuantityList[randomIndex] != 0)
                    {
                        finded = true;
                    }

                }
                //for (int i = 0; i < enemiesQuantityList.Count; i++)
                //{
                //    currentProbably[i] = Mathf.Round((enemiesQuantityList[i] / commonQuantity) * 100);
                //}

                if(GlobalStorage.instance.IsGlobalMode() == true)
                {
                    Debug.Log("1 ERROR");
                }

                EnemiesTypes enemyType = enemiesList[randomIndex];
                GameObject enemy = GlobalStorage.instance.objectsPoolManager.GetEnemy(enemyType);

                enemy.transform.position = randomPosition;
                enemy.SetActive(true);

                //Create boss
                if(ShouldICreateBoss(currentCommonQuantity) == true)
                {
                    enemy.GetComponent<EnemyController>().MakeBoss();
                }

                enemiesOnTheMap.Add(enemy);
                enemiesQuantityList[randomIndex]--;
                currentCommonQuantity--;
                battleUIManager.enemyPart.FillSpawnEnemiesBar(currentCommonQuantity);

                if(GlobalStorage.instance.IsGlobalMode() == true)
                {
                    Debug.Log("2 ERROR");
                }
            }

            yield return waitNextEnemy;
        }
    }

    private bool ShouldICreateBoss(int currentCount)
    {
        for(int i = 0; i < bossBounds.Length; i++)
        {
            if(currentCount <= bossBounds[i].bound && bossBounds[i].wasCreated == false)
            {                
                bossBounds[i].wasCreated = true;
                return true;
            }
        }

        return false;
    }

    private Vector3 GetSpawnPosition()
    {
        int randomIndex = Random.Range(0, spawnPositions.Count);
        Vector3 randomPosition = spawnPositions[randomIndex].transform.position;

        int tempX = Mathf.RoundToInt(Random.Range((randomPosition.x - spawnOffset) * 10, (randomPosition.x + spawnOffset + 1) * 10) / 10);
        int tempY = Mathf.RoundToInt(Random.Range((randomPosition.y - spawnOffset) * 10, (randomPosition.y + spawnOffset + 1) * 10) / 10);

        if (tempX <= 0 + 1 || tempX >= battleMap.GetLength(0) - 1 ||
            tempY <= 0 + 1 || tempY >= battleMap.GetLength(1) - 1)
        {
            return Vector3.zero;
        }

        if (battleMap[tempX, tempY] == false)
        {
            return Vector3.zero;
        }

        int checkGap = 2;
        if ((tempX - checkGap) <= 0 || (tempX + checkGap) >= battleMap.GetLength(0) ||
           (tempY - checkGap) <= 0 || (tempY + checkGap) >= battleMap.GetLength(1))
        {
            return Vector3.zero;
        }
        else
        {
            for (int x = tempX - checkGap / 2; x <= tempX + checkGap / 2; x++)
            {
                for (int y = tempY - checkGap / 2; y <= tempY + checkGap / 2; y++)
                {
                    if (battleMap[tempX, tempY] == false) return Vector3.zero;
                }
            }
        }

        return new Vector3(tempX, tempY, randomPosition.z);
    }

    public void UpdateEnemiesList(GameObject enemy)
    {
        enemiesOnTheMap.Remove(enemy);
        removeQuantity++;

        //end of battle
        if(removeQuantity == maxQuantity)
        {
            canISpawn = false;
            GlobalStorage.instance.battleIUManager.ShowVictoryBlock();
        }
        SpawnControl();        
    }     

    public void SpawnControl()
    {
        //sometimes we have difference between data and i don't know why, i had chacked everithing
        //SOLVED, NEED TO OBSERVE
        if((removeQuantity + currentCommonQuantity + enemiesOnTheMap.Count) != maxQuantity)
        {
            int difference = (removeQuantity + currentCommonQuantity + enemiesOnTheMap.Count) - maxQuantity;

            Debug.Log("CONTROL " + difference);
        }       
    }

    private void OnEnable()
    {
        EventManager.EnemyDestroyed += UpdateEnemiesList;
    }

    private void OnDisable ()
    {
        EventManager.EnemyDestroyed -= UpdateEnemiesList;
    }
}
