using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;

public class EnemyArmyPart : MonoBehaviour
{
    public GameObject uiPanel;
    private EnemyArmyOnTheMap currentEnemyArmy;
    public List<GameObject> currentEnemiesList = new List<GameObject>();
    public List<int> currentEnemiesQuantityList = new List<int>();

    [Header("Small Form")]
    public GameObject smallWindow;
    public TMP_Text captionSmall;
    public Image enemyImage;
    public TMP_Text count;

    [Header("Detail Form")]
    public GameObject detailedWindow;
    public TMP_Text captionDetail;
    public GameObject enemySlotPrefab;
    public GameObject placeForEnemySlots;

    private List<GameObject> allEnemiesList = new List<GameObject>();
    private List<GameObject> allSlotsList = new List<GameObject>();

    private float playerCuriosity;

    public void Init(EnemyArmyOnTheMap enemyArmy)
    {
        if(enemyArmy == null) return;

        currentEnemyArmy = enemyArmy;
        currentEnemiesList = enemyArmy.army.squadList;
        currentEnemiesQuantityList = enemyArmy.army.quantityList;

        uiPanel.SetActive(true);
        Initialize();
    }

    private void Initialize()
    {
        if(currentEnemyArmy != null)
        {
            playerCuriosity = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Curiosity);
            if(allEnemiesList.Count == 0) allEnemiesList = GlobalStorage.instance.enemyManager.finalEnemiesListGO;

            if(playerCuriosity < 1)
                ShowMinimumInfo();
            else
                ShowDetailedInfo();
        }
    }

    private void ShowMinimumInfo()
    {
        detailedWindow.SetActive(false);
        smallWindow.SetActive(true);

        captionSmall.text = "Enemies";

        enemyImage.sprite = currentEnemyArmy.GetComponent<SpriteRenderer>().sprite;
        enemyImage.color = currentEnemyArmy.GetComponent<SpriteRenderer>().color;

        count.text = ConvertQuantity(currentEnemyArmy.commonCount);
    }

    private void ShowDetailedInfo()
    {
        smallWindow.SetActive(false);
        detailedWindow.SetActive(true);

        captionDetail.text = "Enemies";

        FillSlotsField();
    }

    private void FillSlotsField()
    {
        foreach(RectTransform slot in placeForEnemySlots.transform)
        {
            Destroy(slot.gameObject);
            allSlotsList.Clear();
        }

        for(int i = 0; i < currentEnemiesList.Count; i++)
        {
            for(int k = 0; k < allEnemiesList.Count; k++)
            {
                if(currentEnemiesList[i] == allEnemiesList[k])
                {
                    CreateSlot(allEnemiesList[k].GetComponent<EnemyController>(), currentEnemiesQuantityList[i]);
                    break;
                }
            }
        }
    }

    private void CreateSlot(EnemyController enemy, int amount)
    {
        GameObject enemySlot = Instantiate(enemySlotPrefab, placeForEnemySlots.transform);

        EnemySlotUI slotUI = enemySlot.GetComponent<EnemySlotUI>();
        slotUI.Initialize(enemy, amount);
        allSlotsList.Add(enemySlot);
    }

    private string ConvertQuantity(int count)
    {
        string countString = "Small group";

        if(count > 500) countString = "Many";
        if(count > 1000) countString = "Hundreds";
        if(count > 1500) countString = "Hordes";
        if(count > 2000) countString = "Thousands";
        if(count > 3000) countString = "Legion";

        return countString;
    }
}
