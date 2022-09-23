using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;

public class EnemyArmyUI : MonoBehaviour
{
    public GameObject uiPanel;
    public GameObject playersArmyWindow;
    public GameObject realParent;

    private EnemyArmyOnTheMap currentEnemyArmy;
    private List<GameObject> currentEnemiesList = new List<GameObject>();
    private List<int> currentEnemiesQuantityList = new List<int>();
    private bool isOpenedByClick = true;

    [Header("Small Form")]
    public GameObject smallWindow;
    private Image smallWindowImage;
    private Vector2 originalPositionSW;
    private RectTransform rectSmallWindow;
    private Vector2 minAnchorSW;
    private Vector2 maxAnchorSW;

    public TMP_Text captionSmall;
    public Image resourceImage;
    public TMP_Text count;
    public GameObject buttonsPassiveSmall;


    [Header("Detail Form")]
    public GameObject detailedWindow;
    private Image detailedWindowImage;
    private Vector2 originalPositionDW;
    private RectTransform rectDetailedWindow;
    private Vector2 minAnchorDW;
    private Vector2 maxAnchorDW;

    public TMP_Text captionDetail;
    public GameObject buttonsPassiveDetail;
    public GameObject enemySlotPrefab;
    public GameObject placeForEnemySlots;



    private List<GameObject> allEnemiesList = new List<GameObject>();
    private List<GameObject> allSlotsList = new List<GameObject>();

    private float playerCuriosity;

    private void Awake()
    {
        smallWindowImage = smallWindow.GetComponent<Image>();
        rectSmallWindow = smallWindow.GetComponent<RectTransform>();

        detailedWindowImage = detailedWindow.GetComponent<Image>();
        rectDetailedWindow = detailedWindow.GetComponent<RectTransform>();

        SaveOriginalPosition();
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
        playerCuriosity = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Curiosity);

        Initialize();
    }

    public void OpenWithPlayerArmy(EnemyArmyOnTheMap enemyArmy)
    {
        isOpenedByClick = false;
        playerCuriosity = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Curiosity);
        currentEnemyArmy = enemyArmy;
        currentEnemiesList = enemyArmy.currentEnemiesList;
        currentEnemiesQuantityList = enemyArmy.currentEnemiesQuantityList;

        if(playerCuriosity < 1)
        {
            smallWindow.transform.SetParent(playersArmyWindow.transform, false);
            smallWindow.SetActive(true);
            detailedWindow.SetActive(false);

            smallWindowImage.enabled = false;
        }
        else
        {
            detailedWindow.transform.SetParent(playersArmyWindow.transform, false);
            smallWindow.SetActive(false);
            detailedWindow.SetActive(true);

            detailedWindowImage.enabled = false;            
        }

        Initialize();
    }

    private void SaveOriginalPosition()
    {
        originalPositionDW = rectDetailedWindow.anchoredPosition;
        minAnchorDW = rectDetailedWindow.anchorMin;
        maxAnchorDW = rectDetailedWindow.anchorMax;

        originalPositionSW = rectSmallWindow.anchoredPosition;
        minAnchorSW = rectSmallWindow.anchorMin;
        maxAnchorSW = rectSmallWindow.anchorMax;
    }

    private void LoadOriginalPosition()
    {
        rectDetailedWindow.anchoredPosition = originalPositionDW;
        rectDetailedWindow.anchorMin = minAnchorDW;
        rectDetailedWindow.anchorMax = maxAnchorDW;

        rectSmallWindow.anchoredPosition = originalPositionSW;
        rectSmallWindow.anchorMin = minAnchorSW;
        rectSmallWindow.anchorMax = maxAnchorSW;
    }

    private void Initialize()
    {
        if(currentEnemyArmy != null)
        {            
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

        captionSmall.text = "Enemy's army";

        resourceImage.sprite = currentEnemyArmy.GetComponent<SpriteRenderer>().sprite;
        resourceImage.color = currentEnemyArmy.GetComponent<SpriteRenderer>().color;

        count.text = ConvertQuantity(currentEnemyArmy.commonCount);

        buttonsPassiveSmall.SetActive(isOpenedByClick);
    }

    private void ShowDetailedInfo()
    {
        smallWindow.SetActive(false);
        detailedWindow.SetActive(true);

        captionDetail.text = "Enemy's army";

        buttonsPassiveDetail.SetActive(isOpenedByClick);        

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
        GameObject enemySlot = Instantiate(enemySlotPrefab);
        enemySlot.transform.SetParent(placeForEnemySlots.transform, false);
        EnemySlotUI slotUI = enemySlot.GetComponent<EnemySlotUI>();
        slotUI.Initialize(enemy, amount);
        allSlotsList.Add(enemySlot);
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
        GlobalStorage.instance.ModalWindowOpen(false);

        uiPanel.SetActive(false);
    }

    public void CloseWithPlayerArmy()
    {
        LoadOriginalPosition();

        if(playerCuriosity < 1)
        {
            smallWindow.transform.SetParent(realParent.transform, false);
            smallWindowImage.enabled = true;
            smallWindow.SetActive(false);
        }
        else
        {
            detailedWindow.transform.SetParent(realParent.transform, false);
            detailedWindowImage.enabled = true;
            detailedWindow.SetActive(false);
        }
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
        CloseWindow();
    }
}
