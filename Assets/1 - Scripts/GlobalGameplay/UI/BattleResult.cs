using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;

public class BattleResult : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;

    [SerializeField] private GameObject slotPrefab;

    [Header("Reward slots")]
    [SerializeField] private GameObject rewardBlock;
    [SerializeField] private List<GameObject> rewardItemList;
    private List<Image> rewardItemImageList = new List<Image>();
    private List<TMP_Text> rewardItemTextList = new List<TMP_Text>();

    [Header("Losses slots")]
    [SerializeField] private GameObject lossesBlock;
    [SerializeField] private List<GameObject> lossesItemList;
    private List<Image> lossesItemImageList = new List<Image>();
    private List<TMP_Text> lossesItemTextList = new List<TMP_Text>();

    private Dictionary<GameObject, int> lostUnitsDict = new Dictionary<GameObject, int>();

    [Space]
    [Header("Container")]
    public float fullHeight;
    public float losesHeight;
    private RectTransform rectContainer;

    [SerializeField] private CanvasGroup canvas;
    private float currentAlfa = 0;
    private float step = 0.1f;

    public TMP_Text caption;
    private string currentCaption;
    private string retreatCaption = "Retreat!";
    private string defeatCaption = "Defeat!";
    private string victoryCaption = "Victory!";

    //0 - defeat, 1 - victory, -1 - stepback
    private int currentStatus;

    private Reward currentReward;
    private RewardManager rewardManager;
    private ResourcesManager resourcesManager;
    private Dictionary<ResourceType, Sprite> resourcesIcons;

    public void Init(int mode, Army currentArmy)
    {
        GlobalStorage.instance.ModalWindowOpen(true);

        currentStatus = mode;
        canvas.alpha = 0;

        RefactoringContainer();

        if(currentStatus == 1) FillReward(currentArmy);


        uiPanel.SetActive(true);
        StartCoroutine(ShowUI());
    }

    private void FillReward(Army currentArmy)
    {
        currentReward = rewardManager.GetBattleReward(currentArmy);

        for(int i = 0; i < currentReward.resourcesList.Count; i++)
        {
            rewardItemList[i].SetActive(true);
            rewardItemImageList[i].sprite = resourcesIcons[currentReward.resourcesList[i]];
            rewardItemTextList[i].text = currentReward.resourcesQuantity[i].ToString();
        }
    }

    private IEnumerator ShowUI()
    {
        yield return new WaitForSeconds(step * 10);

        WaitForSeconds delay = new WaitForSeconds(step * 0.25f);

        currentAlfa = 0;

        while(currentAlfa < 1)
        {
            currentAlfa += step * 2;
            canvas.alpha = currentAlfa;
            yield return delay;
        }
    }

    private void RegisterDeadUnit(GameObject unit)
    {

    }

    private void DeleteDeadUnit(GameObject unit)
    {

    }

    private void RefactoringContainer()
    {
        if(rectContainer == null)
        {
            rectContainer = canvas.GetComponent<RectTransform>();
            rewardManager = GlobalStorage.instance.rewardManager;
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
            CreateLists();
        }

        ResetItems();

        //-1 - stepback, 0 - defeat, 1 - victory
        float height = 0;

        if(currentStatus == -1)
        {
            height = losesHeight;
            currentCaption = retreatCaption;
            rewardBlock.SetActive(false);
        }

        if(currentStatus == 0)
        {
            height = losesHeight;
            currentCaption = defeatCaption;
            rewardBlock.SetActive(false);
        }

        if(currentStatus == 1)
        {
            height = fullHeight;
            currentCaption = victoryCaption;
            rewardBlock.SetActive(true);
        }

        rectContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        caption.text = currentCaption;
    }

    private void ResetItems()
    {
        foreach(var item in rewardItemList)
            item.SetActive(false);

        foreach(var item in lossesItemList)
            item.SetActive(false);
    }

    private void CreateLists()
    {
        foreach(var item in rewardItemList)
        {
            rewardItemImageList.Add(item.GetComponentInChildren<Image>());
            rewardItemTextList.Add(item.GetComponentInChildren<TMP_Text>());
        }

        foreach(var item in lossesItemList)
        {
            lossesItemImageList.Add(item.GetComponentInChildren<Image>());
            lossesItemTextList.Add(item.GetComponentInChildren<TMP_Text>());
        }
    }

    public void CloseWindow()
    {
        currentReward = null;
        lostUnitsDict.Clear();
        GlobalStorage.instance.ModalWindowOpen(false);
        uiPanel.SetActive(false);
    }

    private void OnEnable()
    { // начало боя - регистрация открыта, клоуз окно - заркыта
        //EventManager.WeLostOneUnit += RegisterDeadUnit;
        //EventManager.RemoveUnitFromInfirmary += RemoveUnitFromInfirmary;
    }

    private void OnDisable()
    {
        //EventManager.WeLostOneUnit -= RegisterDeadUnit;
        //EventManager.RemoveUnitFromInfirmary -= RemoveUnitFromInfirmary;
    }
}
