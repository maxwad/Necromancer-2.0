using FakeDictionaries;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Enums;

public class EnemySpawner : MonoBehaviour
{
    public List<MonoBehaviour> EnemiesOnTheMap => enemiesOnTheMap;

    private ArmyStrength currentStrength;
    private List<EnemiesTypes> enemiesList = new List<EnemiesTypes>();
    private List<int> enemiesQuantityList = new List<int>();
    private int currentCommonQuantity = 0;
    private int maxQuantity = 0;
    private int removeQuantity = 0;
    private List<MonoBehaviour> enemiesOnTheMap = new List<MonoBehaviour>();

    [Space]
    [SerializeField] private EnemyEffector enemyEffector;
    [SerializeField] private Transform spawnPointsGO;
    [SerializeField] private Transform enemyContainerGO;
    [SerializeField] private List<GameObject> spawnPositions;

    [SerializeField] private List<EnumMonoDictionary> enemiesPrefabs;

    [Space]
    private bool canISpawn = false;
    private int enemySlowCount = 100;
    private int enemyTooSlowCount = 150;
    private float spawnOffset = 5f;

    [Space]
    private Coroutine spawnCoroutine;
    private float waitNextEnemyTimeFast = 0.1f;
    private float waitNextEnemyTimeSlow = 0.5f;
    private float waitNextEnemyTimeStop = 2f;
    private WaitForSeconds waitNextEnemyFast;
    private WaitForSeconds waitNextEnemySlow;
    private WaitForSeconds waitNextEnemyStop;
    private WaitForSeconds waitNextEnemy;

    private Transform player;
    private BossData[] bossBounds;
    private bool[,] battleMap;

    private BattleUIManager battleUIManager;
    private ObjectsPoolManager poolManager;


    [Inject]
    public void Construct(
        ObjectsPoolManager poolManager,
        BattleUIManager battleUIManager,
        HeroController player
        )
    {
        this.poolManager = poolManager;
        this.battleUIManager = battleUIManager;
        this.player = player.transform;
    }

    public void Initialize(Army army)
    {
        currentStrength = army.strength;

        enemiesList.Clear();
        foreach(var squad in army.squadList)
        {
            enemiesList.Add(squad);
        }

        enemiesQuantityList.Clear();
        foreach(var squad in army.quantityList)
        {
            enemiesQuantityList.Add(squad);
        }

        waitNextEnemyFast = new WaitForSeconds(waitNextEnemyTimeFast);
        waitNextEnemySlow = new WaitForSeconds(waitNextEnemyTimeSlow);
        waitNextEnemyStop = new WaitForSeconds(waitNextEnemyTimeStop);
        waitNextEnemy = waitNextEnemyFast;

        currentCommonQuantity = 0;

        for(int i = 0; i < enemiesQuantityList.Count; i++)
        {
            currentCommonQuantity += enemiesQuantityList[i];
        }

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
        if(enemiesOnTheMap.Count < enemySlowCount)
            waitNextEnemy = waitNextEnemyFast;

        if(enemiesOnTheMap.Count > enemySlowCount)
            waitNextEnemy = waitNextEnemySlow;

        if(enemiesOnTheMap.Count > enemyTooSlowCount)
            waitNextEnemy = waitNextEnemyStop;
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

        poolManager.DiscardAll(this);
    }

    private IEnumerator SpawnEnemy()
    {
        while(canISpawn == true)
        {
            if(currentCommonQuantity == 0)
            {
                canISpawn = false;
                if(spawnCoroutine != null)
                {
                    StopCoroutine(spawnCoroutine);
                }

                break;
            }

            spawnPointsGO.position = player.position;

            Vector3 randomPosition = GetSpawnPosition();

            if(randomPosition != Vector3.zero)
            {
                int randomIndex = 0;
                bool finded = false;

                while(finded == false && currentCommonQuantity != 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, enemiesList.Count);
                    if(enemiesQuantityList[randomIndex] != 0)
                    {
                        finded = true;
                    }

                }
                //for (int i = 0; i < enemiesQuantityList.Count; i++)
                //{
                //    currentProbably[i] = Mathf.Round((enemiesQuantityList[i] / commonQuantity) * 100);
                //}

                EnemiesTypes enemyType = enemiesList[randomIndex];

                MonoBehaviour enemyPrefab = enemiesPrefabs.First(e => e.key == enemyType).value;

                MonoBehaviour enemy = poolManager.GetOrCreateElement(enemyPrefab, this, enemyContainerGO, true);

                enemy.transform.position = randomPosition;

                //Create boss
                if(ShouldICreateBoss(currentCommonQuantity))
                {
                    enemy.GetComponent<EnemyController>().MakeBoss();
                }

                enemiesOnTheMap.Add(enemy);
                enemiesQuantityList[randomIndex]--;
                currentCommonQuantity--;
                battleUIManager.enemyPart.FillSpawnEnemiesBar(currentCommonQuantity);

                if(GlobalStorage.instance.IsGlobalMode())
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
        int randomIndex = UnityEngine.Random.Range(0, spawnPositions.Count);
        Vector3 randomPosition = spawnPositions[randomIndex].transform.position;

        int tempX = Mathf.RoundToInt(UnityEngine.Random.Range((randomPosition.x - spawnOffset) * 10, (randomPosition.x + spawnOffset + 1) * 10) / 10);
        int tempY = Mathf.RoundToInt(UnityEngine.Random.Range((randomPosition.y - spawnOffset) * 10, (randomPosition.y + spawnOffset + 1) * 10) / 10);

        if(tempX <= 0 + 1 || tempX >= battleMap.GetLength(0) - 1 ||
            tempY <= 0 + 1 || tempY >= battleMap.GetLength(1) - 1)
        {
            return Vector3.zero;
        }

        if(battleMap[tempX, tempY] == false)
        {
            return Vector3.zero;
        }

        int checkGap = 2;
        if((tempX - checkGap) <= 0 || (tempX + checkGap) >= battleMap.GetLength(0) ||
           (tempY - checkGap) <= 0 || (tempY + checkGap) >= battleMap.GetLength(1))
        {
            return Vector3.zero;
        }
        else
        {
            for(int x = tempX - checkGap / 2; x <= tempX + checkGap / 2; x++)
            {
                for(int y = tempY - checkGap / 2; y <= tempY + checkGap / 2; y++)
                {
                    if(battleMap[tempX, tempY] == false) return Vector3.zero;
                }
            }
        }

        return new Vector3(tempX, tempY, randomPosition.z);
    }

    public void UpdateEnemiesList(MonoBehaviour enemy)
    {
        enemiesOnTheMap.Remove(enemy);
        removeQuantity++;

        //end of battle
        if(removeQuantity == maxQuantity)
        {
            canISpawn = false;
            battleUIManager.ShowVictoryBlock();
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

    private void OnDisable()
    {
        EventManager.EnemyDestroyed -= UpdateEnemiesList;
    }
}
