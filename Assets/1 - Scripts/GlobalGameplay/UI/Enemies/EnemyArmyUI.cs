using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;

public class EnemyArmyUI : MonoBehaviour
{
    public GameObject uiPanel;
    public CanvasGroup canvas;
    public GameObject playersArmyWindow;
    public GameObject realParent;

    private EnemyArmyOnTheMap currentEnemyArmy;
    private List<GameObject> currentEnemiesList = new List<GameObject>();
    private List<int> currentEnemiesQuantityList = new List<int>();
    private bool isOpenedByClick = true;

    [Header("Small Form")]
    public GameObject smallWindow;
    public TMP_Text captionSmall;
    public Image resourceImage;
    public TMP_Text count;
    public GameObject buttonsPassiveSmall;


    [Header("Detail Form")]
    public GameObject detailedWindow;
    public TMP_Text captionDetail;
    public GameObject buttonsPassiveDetail;
    public GameObject enemySlotPrefab;
    public GameObject placeForEnemySlots;

    private List<GameObject> allEnemiesList = new List<GameObject>();
    private List<GameObject> allSlotsList = new List<GameObject>();

    private float playerCuriosity;

    public void OpenWindow(bool modeClick, EnemyArmyOnTheMap enemyArmy = null)
    {
        //modeClick = false - by movement; true - by click

        GlobalStorage.instance.ModalWindowOpen(true);
        isOpenedByClick = modeClick;

        currentEnemyArmy = enemyArmy;
        currentEnemiesList = enemyArmy.army.squadList;
        currentEnemiesQuantityList = enemyArmy.army.quantityList;

        uiPanel.SetActive(true);
        if(canvas == null) canvas = uiPanel.GetComponent<CanvasGroup>();
        Fading.instance.Fade(true, canvas);

        playerCuriosity = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Curiosity);

        Initialize();
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

        captionSmall.text = "Enemies";

        resourceImage.sprite = currentEnemyArmy.GetComponent<SpriteRenderer>().sprite;
        resourceImage.color = currentEnemyArmy.GetComponent<SpriteRenderer>().color;

        count.text = ConvertQuantity(currentEnemyArmy.commonCount);

        buttonsPassiveSmall.SetActive(isOpenedByClick);
    }

    private void ShowDetailedInfo()
    {
        smallWindow.SetActive(false);
        detailedWindow.SetActive(true);

        captionDetail.text = "Enemies";

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

    //public void ToTheBattle()
    //{
    //    CloseWindow();
    //    GlobalStorage.instance.battleManager.InitializeBattle();
    //}

    //public void AutoBattle()
    //{
    //    GlobalStorage.instance.battleManager.AutoBattle();
    //}

    //public void StepBack()
    //{
    //    CloseWindow();
    //}
}
