using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class EnemySpawner : MonoBehaviour
{
    private List<GameObject> enemiesList;
    private List<int> enemiesQuantityList;
    private int commonQuantity = 0;
    private int maxQuantity = 0;
    private int removeQuantity = 0;
    //[SerializeField] private GameObject enemiesContainer;
    public List<GameObject> enemiesOnTheMap = new List<GameObject>();

    [Space]
    [SerializeField] private List<GameObject> spawnPositions;

    [Space]
    private bool canISpawn = false;
    private int enemySlowCount = 100;
    private int enemyTooSlowCount = 150;
    private float spawnOffset = 5f;

    private bool isBossCreated = false;

    [Space]
    private Coroutine spawnCoroutine;
    private float waitNextEnemyTimeFast = 0.1f;
    private float waitNextEnemyTimeSlow = 0.5f;
    private float waitNextEnemyTimeStop = 2f;
    private WaitForSeconds waitNextEnemyFast;
    private WaitForSeconds waitNextEnemySlow;
    private WaitForSeconds waitNextEnemyStop;
    private WaitForSeconds waitNextEnemy;

    //private BattleMap battleMap;

    private bool[,] battleMap;

    public void Initialize(List<GameObject> enemiesPrefabs, List<int> quantity)
    {
        enemiesList = enemiesPrefabs;
        enemiesQuantityList = quantity;
        waitNextEnemyFast = new WaitForSeconds(waitNextEnemyTimeFast);
        waitNextEnemySlow = new WaitForSeconds(waitNextEnemyTimeSlow);
        waitNextEnemyStop = new WaitForSeconds(waitNextEnemyTimeStop);
        waitNextEnemy = waitNextEnemyFast;

        commonQuantity = 0;

        for(int i = 0; i < enemiesQuantityList.Count; i++)
            commonQuantity += enemiesQuantityList[i];

        maxQuantity = commonQuantity;

        EventManager.OnEnemiesCountEvent(commonQuantity);
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
    }

    public void StopSpawnEnemy()
    {
        canISpawn = false;
        isBossCreated = false;
        enemiesOnTheMap.Clear();
    }

    private IEnumerator SpawnEnemy()
    {

        //List<float> currentProbably = new List<float>();

        while (canISpawn == true)
        {      
            if (commonQuantity == 0)
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

                while (finded == false && commonQuantity != 0)
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

                EnemiesTypes enemyType = enemiesList[randomIndex].GetComponent<EnemyController>().enemiesType;
                GameObject enemy = GlobalStorage.instance.objectsPoolManager.GetObjectFromPool(ObjectPool.Enemy, enemyType);
                enemy.transform.position = randomPosition;
                enemy.SetActive(true);

                //Create boss
                if(commonQuantity <= maxQuantity / 2 && isBossCreated == false)
                {
                    enemy.GetComponent<EnemyController>().MakeBoss();
                    isBossCreated = true;
                }

                enemiesOnTheMap.Add(enemy);
                enemiesQuantityList[randomIndex]--;
                commonQuantity--;
            }

            yield return waitNextEnemy;
        }
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
        if((removeQuantity + commonQuantity + enemiesOnTheMap.Count) != maxQuantity)
        {
            int difference = (removeQuantity + commonQuantity + enemiesOnTheMap.Count) - maxQuantity;

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
