using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;

public class EnemyArmyUI : MonoBehaviour
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
    public GameObject enemyCardPrefab;
    public GameObject enemySlotPrefab;
    public GameObject placeForEnemySlots;
    public GameObject placeForEnemyCard;
    public GameObject autoBattleButton;


    private List<GameObject> allEnemiesList = new List<GameObject>();
    private List<GameObject> allSlotsList = new List<GameObject>();
    private List<GameObject> allCardsList = new List<GameObject>();



    private float playerCuriosity;

    public void OpenWindow(bool modeClick, EnemyArmyOnTheMap enemyArmy = null)
    {
        //modeClick = false - by movement; true - by click

        GlobalStorage.instance.isModalWindowOpen = true;
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

            if(allSlotsList.Count == 0) CreateAllSlots();
            
            if(playerCuriosity < 2) 
                ShowMinimumInfo();
            else
                ShowDetailedInfo();
        }
    }

    private void ShowMinimumInfo()
    {
        detailedWindow.SetActive(false);
        smallWindow.SetActive(true);

        captionSmall.text = currentEnemyArmy.gameObject.name;

        resourceImage.sprite = currentEnemyArmy.GetComponent<SpriteRenderer>().sprite;
        resourceImage.color = currentEnemyArmy.GetComponent<SpriteRenderer>().color;

        if(playerCuriosity == 1)
            count.text = currentEnemyArmy.commonCount.ToString();
        else
            count.text = ConvertQuantity(currentEnemyArmy.commonCount);

        if(isOpenedByClick == true)
        {
            buttonsPassiveSmall.SetActive(true);
            buttonsActiveSmall.SetActive(false);
        }
        else
        {
            buttonsPassiveSmall.SetActive(false);
            buttonsActiveSmall.SetActive(true);
        }
    }

    private void ShowDetailedInfo()
    {
        smallWindow.SetActive(false);
        detailedWindow.SetActive(true);

        captionDetail.text = currentEnemyArmy.gameObject.name;

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

            if(playerCuriosity == 3)
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
            slot.gameObject.SetActive(false);
        }

        foreach(RectTransform card in placeForEnemyCard.transform)
        {
            card.gameObject.SetActive(false);
        }

        for(int i = 0; i < currentEnemiesList.Count; i++)
        {
            for(int k = 0; k < allEnemiesList.Count; k++)
            {
                if(allEnemiesList[k] == currentEnemiesList[i])
                {
                    allSlotsList[k].SetActive(true);
                    allSlotsList[k].GetComponent<EnemySlotUI>().SetAmount(currentEnemiesQuantityList[i]);
                    break;
                }
            }
        }
    }

    public void ShowDetails(int index)
    {
        bool showMode = playerCuriosity > 2 ? true : false;
        foreach(var card in allCardsList)
        {
            card.SetActive(false);
        }

        allCardsList[index].SetActive(true);
        allCardsList[index].GetComponent<EnemyCardUI>().Initialize(null, showMode);
    }

    private void CreateAllSlots()
    {
        allEnemiesList = GlobalStorage.instance.battleManager.allEnemiesList;
        for(int i = 0; i < allEnemiesList.Count; i++)
        {
            EnemyController enemy = allEnemiesList[i].GetComponent<EnemyController>();

            EnemyCardUI card = CreateCard(enemy);
            GameObject enemySlot = Instantiate(enemySlotPrefab, placeForEnemySlots.transform);

            EnemySlotUI slotUI = enemySlot.GetComponent<EnemySlotUI>();
            slotUI.Initialize(i, enemy.icon, card, enemy.enemiesType);
            enemySlot.SetActive(false);
            allSlotsList.Add(enemySlot);
        }
    }

    private EnemyCardUI CreateCard(EnemyController enemy)
    {
        GameObject newCardGO = Instantiate(enemyCardPrefab, placeForEnemyCard.transform);
        EnemyCardUI card = newCardGO.GetComponent<EnemyCardUI>();
        card.Initialize(enemy);
        newCardGO.SetActive(false);
        allCardsList.Add(newCardGO);

        return card;
    }

    private string ConvertQuantity(int count)
    {
        string countString = "Small group";

        if(count > 500 ) countString = "Many";
        if(count > 1000 ) countString = "Hundrets";
        if(count > 1500 ) countString = "Hordes";
        if(count > 2000 ) countString = "Thousands";
        if(count > 3000 ) countString = "Legion";

        return countString;
    }

    public void CloseWindow()
    {
        GlobalStorage.instance.isModalWindowOpen = false;

        uiPanel.SetActive(false);
    }

    public void ToTheBattle()
    {
        CloseWindow();
        GlobalStorage.instance.battleManager.InitializeBattle();
    }

    public void AutoBattle()
    {
        GlobalStorage.instance.battleManager.AutoBattle();
    }

    public void StepBack()
    {
        GlobalStorage.instance.globalPlayer.GetComponent<GMPlayerMovement>().StepBack();
        CloseWindow();
    }

    public void ToTheHeroArmy()
    {
        GlobalStorage.instance.playerManager.playersArmyWindow.OpenFromBattleWindow();
    }
}
