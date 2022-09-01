using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static NameManager;

public class BattleUIManager : MonoBehaviour
{
    public Canvas uiCanvas;
    private PlayerStats playerStats;
    private ResourcesManager resourcesManager;
    private bool isBattleOver = false;

    [Header("Left Column Exp")]
    [SerializeField] private RectTransform currentScaleValue;
    private Image currentScaleValueImage;
    [SerializeField] private Color levelUpColor;
    [SerializeField] private Color normalLevelColor;

    [Header("Rigth Column Exp")]
    [SerializeField] private GameObject tempLevelGO;
    [SerializeField] private RectTransform currentTempLevelWrapper;
    private float heigthOneLevel;
    private float currentMaxLevel;
    private List<GameObject> levelList = new List<GameObject>();

    [Header("Exp Effects")]
    private float blinkTime = 0.005f;
    private Coroutine blinkOneCoroutine;

    [Header("Infirmary")]
    [SerializeField] private RectTransform infirmaryValue;
    [SerializeField] private TMP_Text infirmaryInfo;
    private float currentMaxInfirmaryCount;
    private float currentInfirmaryCount;
    [SerializeField] private Color infirmaryUpColor;
    [SerializeField] private Color infirmaryDownColor;
    [SerializeField] private Color normalInfirmaryColor;

    [Header("Mana")]
    [SerializeField] private RectTransform manaValue;
    [SerializeField] private TMP_Text manaInfo;
    private float currentMaxManaCount;
    private float currentManaCount;
    [SerializeField] private Color manaUpColor;
    [SerializeField] private Color manaDownColor;
    [SerializeField] private Color normalManaColor;

    [Header("Gold")]
    [SerializeField] private TMP_Text goldInfo;
    //private float currentGoldCount;

    [Header("Spells")]
    [SerializeField] private Button buttonSpell;
    private List<SpellStat> currentSpells = new List<SpellStat>();
    [SerializeField] private GameObject spellButtonContainer;
    private List<Button> currentSpellsButtons = new List<Button>();
    private int countOfActiveSpells = 5;
    private int currentSpellIndex = -1;

    [Header("Enemy")]
    [SerializeField] private RectTransform enemiesWrapper;
    [SerializeField] private RectTransform enemiesValue;
    [SerializeField] private TMP_Text enemiesInfo;
    private float maxEnemiesCount = 0;
    private float currentEnemiesCount = 0;

    [Header("End of Battle")]
    [SerializeField] private GameObject leaveBlock;
    private bool isLeaveBlockOpened = false;
    [SerializeField] private GameObject victoryBlock;
    [SerializeField] private CanvasGroup victoryCanvasGroup;
    [SerializeField] private GameObject defeatBlock;
    [SerializeField] private CanvasGroup defeatCanvasGroup;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            if(GlobalStorage.instance.isGlobalMode == false) OpenLeaveBlock(!isLeaveBlockOpened);
        }

        if(GlobalStorage.instance.isGlobalMode == false) Spelling();
    }

    private void Spelling()
    {
        switch(Input.inputString)
        {
            case "1":
                currentSpellIndex = 0;
                break;
            case "2":
                currentSpellIndex = 1;
                break;
            case "3":
                currentSpellIndex = 2;
                break;
            case "4":
                currentSpellIndex = 3;
                break;
            case "5":
                currentSpellIndex = 4;
                break;
            case "6":
                currentSpellIndex = 5;
                break;
            case "7":
                currentSpellIndex = 6;
                break;
            case "8":
                currentSpellIndex = 7;
                break;
            case "9":
                currentSpellIndex = 8;
                break;
            case "0":
                currentSpellIndex = 9;
                break;

            default:
                currentSpellIndex = -1;
                break;
        }

        if(currentSpellIndex != -1) currentSpellsButtons[currentSpellIndex].GetComponent<SpellButtonController>().ActivateSpell();
    }

    public void Inizialize(bool mode)
    {
        playerStats = GlobalStorage.instance.playerStats;
        resourcesManager = GlobalStorage.instance.resourcesManager;

        uiCanvas.gameObject.SetActive(!mode);

        currentScaleValueImage = currentScaleValue.GetComponent<Image>();
        isBattleOver = false;

        if(mode == false) ResetCanvas();
    }

    #region Helpers
    private void ResetCanvas()
    {
        FillRigthTempLevelScale();

        FillInfirmary();

        FillMana();

        FillGold();

        FillSpells(-1);

        FillEnemiesBar(null);
    }

    private void Blink(bool mode, Image panel, Color effectColor, Color normalColor, float divider = 5)
    {
        //mode: true - one coroutines at the time, false - few coroutines at the time

        panel.color = effectColor;

        if(mode == true)
        {
            if(blinkOneCoroutine != null) StopCoroutine(blinkOneCoroutine);

            blinkOneCoroutine = StartCoroutine(ColorBack(panel, normalColor, divider));
        }
        else
        {
            StartCoroutine(ColorBack(panel, normalColor, divider));
        }

    }

    private IEnumerator ColorBack(Image panel, Color normalColor, float divider)
    {
        //the bigger divider the slower animation

        float time = 0;

        while(panel.color != normalColor)
        {
            time += Time.deltaTime;
            panel.color = Color.Lerp(panel.color, normalColor, time / divider);
            yield return new WaitForSeconds(blinkTime);
        }
    }

    public void OpenLeaveBlock(bool mode)
    {
        if(isBattleOver == true) return;

        isLeaveBlockOpened = mode;
        GlobalStorage.instance.isModalWindowOpen = mode;
        MenuManager.instance.MiniPause(mode);
        leaveBlock.SetActive(mode);
    }

    public void LeaveTheBattle()
    {
        OpenLeaveBlock(false);
        GlobalStorage.instance.battleManager.FinishTheBattle(-1);
    }

    public void ShowVictoryBlock()
    {
        isBattleOver = true;
        StartCoroutine(VictoryBlockAppearing());

        IEnumerator VictoryBlockAppearing()
        {
            victoryBlock.SetActive(true);
            victoryCanvasGroup.alpha = 0;

            while(victoryCanvasGroup.alpha < 1)
            {
                victoryCanvasGroup.alpha += 0.01f;
                yield return null;
            }
        }
    }

    public void Victory()
    {
        victoryBlock.SetActive(false);
        GlobalStorage.instance.battleManager.FinishTheBattle(1);
    }

    public void ShowDefeatBlock()
    {
        isBattleOver = true;
        StartCoroutine(DefeatBlockAppearing());

        IEnumerator DefeatBlockAppearing()
        {
            defeatBlock.SetActive(true);
            defeatCanvasGroup.alpha = 0;

            while(defeatCanvasGroup.alpha < 1)
            {
                defeatCanvasGroup.alpha += 0.01f;
                yield return null;
            }
        }
    }

    public void Defeat()
    {
        defeatBlock.SetActive(false);
        GlobalStorage.instance.battleManager.FinishTheBattle(0);
    }

    #endregion

    #region TempExp
    private void FillRigthTempLevelScale()
    {
        levelList.Clear();

        foreach(Transform child in currentTempLevelWrapper.transform)
            Destroy(child.gameObject);

        currentScaleValueImage.fillAmount = 0;
        currentMaxLevel = playerStats.GetStartParameter(PlayersStats.Level);

        heigthOneLevel = currentTempLevelWrapper.rect.height / currentMaxLevel;

        for(int i = 0; i < currentMaxLevel; i++)
        {
            GameObject levelPart = Instantiate(tempLevelGO);

            RectTransform rectLevelSize = levelPart.GetComponent<RectTransform>();
            levelPart.transform.SetParent(currentTempLevelWrapper.transform);
            rectLevelSize.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heigthOneLevel);
            rectLevelSize.anchoredPosition = new Vector2(0, heigthOneLevel / 2 + heigthOneLevel * i);

            RectTransform rectNumb = levelPart.GetComponentsInChildren<Image>()[1].GetComponent<RectTransform>();
            rectNumb.anchoredPosition = new Vector2(0, -rectNumb.rect.height / 2);

            rectNumb.GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();

            levelList.Add(levelPart);
        }
    }

    public void UpgradeScale(float scale, float value)
    {
        currentScaleValueImage.fillAmount = value / scale;
        Blink(true, currentScaleValueImage, levelUpColor, normalLevelColor);
    }

    public void TempLevelUp(float oldLevel)
    {
        levelList[(int)oldLevel].GetComponent<Image>().enabled = true;

        if(oldLevel + 1 < levelList.Count)
            currentScaleValueImage.fillAmount = 0;

        foreach(var itemLevel in levelList)
        {
            Image imageLevel = itemLevel.GetComponent<Image>();

            if(imageLevel.enabled == true) Blink(false, imageLevel, levelUpColor, normalLevelColor, 100);
        }
    }

    #endregion

    #region Infirmary

    private void FillInfirmary(float max = 0, float current = 0)
    {
        Color blinkColor = infirmaryUpColor;
        if(max == 0)
        {
            currentMaxInfirmaryCount = playerStats.GetStartParameter(PlayersStats.Infirmary);
            currentInfirmaryCount = GlobalStorage.instance.infirmaryManager.injuredList.Count;
        }
        else
        {
            if(currentInfirmaryCount > current) blinkColor = infirmaryDownColor;
            currentMaxInfirmaryCount = max;            
            currentInfirmaryCount = current;
        }

        Image infirmaryScale = infirmaryValue.GetComponent<Image>();

        float widthInfirmary = currentInfirmaryCount / currentMaxInfirmaryCount;

        infirmaryScale.fillAmount = widthInfirmary;
        infirmaryInfo.text = currentInfirmaryCount.ToString() + "/" + currentMaxInfirmaryCount.ToString();
        Blink(false, infirmaryScale, blinkColor, normalInfirmaryColor);
    }

    private void UpdateInfirmaryUI(float quantity, float capacity)
    {
        FillInfirmary(capacity, quantity);        
    }

    #endregion

    #region Mana

    private void FillMana(float max = 0, float current = 0)
    {
        if(playerStats == null) playerStats = GlobalStorage.instance.playerStats;

        Color blinkColor = manaUpColor;

        if(max == 0)
        {
            currentMaxManaCount = playerStats.GetStartParameter(PlayersStats.Mana);
            currentManaCount = playerStats.GetCurrentParameter(PlayersStats.Mana); 
        }
        else
        {
            if(currentManaCount > current) blinkColor = manaDownColor;
            currentMaxManaCount = max;
            currentManaCount = current;
        }

        Image manaScale = manaValue.GetComponent<Image>();
        float widthMana = currentManaCount / currentMaxManaCount;

        manaScale.fillAmount = widthMana;
        manaInfo.text = currentManaCount.ToString();

        Blink(false, manaScale, blinkColor, normalManaColor, 10);
    }

    private void UpdateManaUI(PlayersStats stat, float maxValue, float currentValue)
    {
        if(stat == PlayersStats.Mana) FillMana(maxValue, currentValue);
    }

    #endregion

    #region Gold
    private void FillGold()
    {
        if(resourcesManager == null) resourcesManager = GlobalStorage.instance.resourcesManager;

        goldInfo.text = resourcesManager.GetAllResources()[ResourceType.Gold].ToString();
    }

    #endregion

    #region Spells 

    private void FillSpells(int numberOfSpell)
    {
        currentSpells = GlobalStorage.instance.spellManager.GetCurrentSpells();

        if(numberOfSpell == -1)
        {
            foreach(Transform child in spellButtonContainer.transform)
                Destroy(child.gameObject);

            currentSpellsButtons.Clear();

            for(int i = 0; i < countOfActiveSpells; i++)
            {
                int slotNumber = -1;
                if(i < countOfActiveSpells)
                {
                    slotNumber = i + 1;
                    if(i + 1 == 10) slotNumber = 0;
                }   

                Button button = Instantiate(buttonSpell);
                button.GetComponent<SpellButtonController>().InitializeButton(currentSpells[i], slotNumber);
                currentSpellsButtons.Add(button);
                button.transform.SetParent(spellButtonContainer.transform);
            }
        }
        else
        {
            for(int i = 0; i < countOfActiveSpells; i++)
            {
                if(i == numberOfSpell)
                {
                    Button button = Instantiate(buttonSpell);
                    button.GetComponent<SpellButtonController>().InitializeButton(currentSpells[i]);
                    currentSpellsButtons.Add(button);
                    button.transform.SetParent(spellButtonContainer.transform);

                    break;
                }
            }
        }        
    }

    #endregion

    #region EnemiesBar
    private void GetStartCountEnemies(int count)
    {
        maxEnemiesCount = count;
        currentEnemiesCount = count;
    }

    public void FillEnemiesBar(GameObject enemy)
    {
        if(enemy != null) currentEnemiesCount--;

        float widthEnemyScale = (maxEnemiesCount - currentEnemiesCount) / maxEnemiesCount;

        enemiesValue.GetComponent<Image>().fillAmount = widthEnemyScale;
        enemiesInfo.text = currentEnemiesCount.ToString();
    }

    #endregion

    private void OnEnable()
    {
        EventManager.ChangePlayer             += Inizialize;
        EventManager.UpdateInfirmaryUI        += UpdateInfirmaryUI;
        EventManager.UpgradeStatCurrentValue  += UpdateManaUI;
        EventManager.UpgradeResources         += FillGold;
        EventManager.EnemiesCount             += GetStartCountEnemies;
        EventManager.EnemyDestroyed           += FillEnemiesBar;
    }

    private void OnDisable()
    {
        EventManager.ChangePlayer             -= Inizialize;
        EventManager.UpdateInfirmaryUI        -= UpdateInfirmaryUI;
        EventManager.UpgradeStatCurrentValue  -= UpdateManaUI;
        EventManager.UpgradeResources         -= FillGold;
        EventManager.EnemiesCount             -= GetStartCountEnemies;
        EventManager.EnemyDestroyed           -= FillEnemiesBar;
    }
}
