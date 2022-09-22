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
    private bool isOpenedByClick = true;

    [Header("Small Form")]
    public GameObject smallWindow;
    public TMP_Text captionSmall;
    public Image resourceImage;
    public TMP_Text count;
    public GameObject buttonsPassiveSmall;
    public GameObject buttonsActiveSmall;

    [Header("Detail Form")]
    public GameObject detailedWindow;
    public TMP_Text captionDetail;
    public GameObject buttonsPassiveDetail;
    public GameObject buttonsActiveDetail;
    //public GameObject enemyCardPrefab;
    public GameObject enemySlotPrefab;
    public GameObject placeForEnemySlots;
    //public GameObject placeForEnemyCard;
    public GameObject autoBattleButton;


    private List<GameObject> allEnemiesList = new List<GameObject>();
    private List<GameObject> allSlotsList = new List<GameObject>();
    //private List<GameObject> allCardsList = new List<GameObject>();
    //private List<GameObject> currentSlotsList = new List<GameObject>();


    private float playerCuriosity;

    public void Init()
    {

    }

    public void OpenWindow(bool modeClick, EnemyArmyOnTheMap enemyArmy = null)
    {
        //modeClick = false - by movement; true - by click

        GlobalStorage.instance.ModalWindowOpen(true);
        isOpenedByClick = modeClick;

        currentEnemyArmy = enemyArmy;
        currentEnemiesList = enemyArmy.currentEnemiesList;
        currentEnemiesQuantityList = enemyArmy.currentEnemiesQuantityList;

        uiPanel.SetActive(true);
        Initialize();
    }

    private void Initialize()
    {
        if(currentEnemyArmy != null)
        {
            playerCuriosity = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Curiosity);
            if(allEnemiesList.Count == 0) allEnemiesList = GlobalStorage.instance.enemyManager.finalEnemiesListGO;

            //if(playerCuriosity < 1)
            //    ShowMinimumInfo();
            //else
            //    ShowDetailedInfo();
        }
    }

    private void ShowDetailedInfo()
    {
        smallWindow.SetActive(false);
        detailedWindow.SetActive(true);

        captionDetail.text = "Enemy";

        if(isOpenedByClick == true)
        {
            buttonsPassiveDetail.SetActive(true);
            buttonsActiveDetail.SetActive(false);
            autoBattleButton.SetActive(false);
        }
        else
        {
            buttonsPassiveDetail.SetActive(false);
            buttonsActiveDetail.SetActive(true);

            if(playerCuriosity == 4)
                autoBattleButton.SetActive(true);
            else
                autoBattleButton.SetActive(true);
        }

        FillSlotsField();
    }

    private void FillSlotsField()
    {
        foreach(RectTransform slot in placeForEnemySlots.transform)
        {
            Destroy(slot.gameObject);
            allSlotsList.Clear();
        }

        //foreach(RectTransform card in placeForEnemyCard.transform)
        //{
        //    Destroy(card.gameObject);
        //    allCardsList.Clear();
        //}

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
        //EnemyCardUI card = CreateCard(enemy);
        GameObject enemySlot = Instantiate(enemySlotPrefab, placeForEnemySlots.transform);

        EnemySlotUI slotUI = enemySlot.GetComponent<EnemySlotUI>();
        slotUI.Initialize(enemy, amount);
        allSlotsList.Add(enemySlot);
    }

    private string ConvertQuantity(int count)
    {
        string countString = "Small group";

        if(count > 500) countString = "Many";
        if(count > 1000) countString = "Hundrets";
        if(count > 1500) countString = "Hordes";
        if(count > 2000) countString = "Thousands";
        if(count > 3000) countString = "Legion";

        return countString;
    }
}
