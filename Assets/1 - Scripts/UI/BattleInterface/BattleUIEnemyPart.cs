using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Enums;

public class BattleUIEnemyPart : MonoBehaviour
{
    private BattleUIManager battleUIManager;

    [Header("Enemy")]
    [SerializeField] private RectTransform bossSpawnWrapper;
    [SerializeField] private Image enemiesValue;
    [SerializeField] private Image spawnValue;
    [SerializeField] private TMP_Text enemiesInfo;
    private float maxEnemiesCount = 0;
    private float currentEnemiesCount = 0;

    private float bossIndex = 0;
    [SerializeField] private GameObject bossMark;
    [SerializeField] private GameObject bossUI;
    [SerializeField] private RectTransform bossUIItemWrapper;
    private List<GameObject> bossMarkList = new List<GameObject>();
    private BossData[] bosses;
    public class BossUIData
    {
        public BossController boss;
        public Image bossMark;
        public RuneSO rune;
        public BattleBossUI bossUI;
    }
    private Dictionary<BossController, BossUIData> bossDict = new Dictionary<BossController, BossUIData>();

    public void Init(BattleUIManager manager)
    {
        battleUIManager = manager;
    }

    public void SetStartEnemiesParameters(int count, BossData[] bossesData)
    {
        maxEnemiesCount = count;
        currentEnemiesCount = count;

        bosses = bossesData;
        bossIndex = 0;

        foreach(var boss in bossDict)
        {
            UnRegisterBoss(boss.Key, false);
        }
        bossDict.Clear();

        foreach(GameObject child in bossMarkList)
            Destroy(child);

        bossMarkList.Clear();
        float wrapperWidth = bossSpawnWrapper.rect.width;

        for(int i = 0; i < bosses.Length; i++)
        {
            GameObject bossItem = Instantiate(bossMark);
            bossItem.transform.SetParent(bossSpawnWrapper.transform, false);

            float wPosition = (maxEnemiesCount - bosses[i].bound) / maxEnemiesCount;
            bossItem.GetComponent<RectTransform>().localPosition = new Vector3(wrapperWidth - wPosition * wrapperWidth, 0, 0);

            bossMarkList.Add(bossItem);
        }
    }

    public Image ActivateBossMark()
    {
        Image mark = null;

        if(bossIndex < bossMarkList.Count)
        {
            mark = bossMarkList[(int)bossIndex].GetComponent<Image>();
            mark.color = Color.white;
        }

        bossIndex++;

        return mark;
    }

    public void FillSpawnEnemiesBar(int currentQuantity)
    {
        float widthEnemyScale = (maxEnemiesCount - currentQuantity) / maxEnemiesCount;
        spawnValue.fillAmount = widthEnemyScale;
    }

    public void FillDeadEnemiesBar(GameObject enemy)
    {
        if(enemy != null) currentEnemiesCount--;

        float widthEnemyScale = (maxEnemiesCount - currentEnemiesCount) / maxEnemiesCount;

        enemiesValue.fillAmount = widthEnemyScale;
        enemiesInfo.text = currentEnemiesCount.ToString();
    }

    public void RegisterBoss(float health, BossController bossC)
    {
        Image mark = ActivateBossMark();
        GameObject bossUIGO = Instantiate(bossUI);
        bossUIGO.transform.SetParent(bossUIItemWrapper, false);
        BattleBossUI bossUIConmponent = bossUIGO.GetComponent<BattleBossUI>();

        string tip = bossC.rune.positiveDescription.Replace("$", bossC.rune.value.ToString());
        bossUIConmponent.Init(bossC.sprite, bossC.rune.activeIcon, tip, health);

        bossDict[bossC] = new BossUIData
        {
            boss = bossC,
            bossMark = mark,
            rune = bossC.rune,
            bossUI = bossUIConmponent
        };
    }

    public void UnRegisterBoss(BossController boss, bool cleanningMode)
    {
        Destroy(bossDict[boss].bossMark.gameObject);
        Destroy(bossDict[boss].bossUI.gameObject);

        if(cleanningMode == true) bossDict.Remove(boss);
    }

    public void UpdateBossHealth(float currentHealth, BossController boss)
    {
        bossDict[boss].bossUI.UpdateHealth(currentHealth);
    }

    private void OnEnable()
    {
        EventManager.EnemyDestroyed += FillDeadEnemiesBar;
    }

    private void OnDisable()
    {
        EventManager.EnemyDestroyed -= FillDeadEnemiesBar;
    }
}
