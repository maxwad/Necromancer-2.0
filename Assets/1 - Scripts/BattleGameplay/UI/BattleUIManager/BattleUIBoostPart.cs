using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;
using System;

public class BattleUIBoostPart : MonoBehaviour
{
    private BattleUIManager battleUIManager;
    private BattleBoostManager boostManager;
    private ObjectsPoolManager objectsPool;

    [Header("Boost")]
    [SerializeField] private RectTransform unitsBoostWrapper;
    [SerializeField] private RectTransform enemiesBoostWrapper;
    [SerializeField] private RectTransform fleshBoostWrapper;
    [SerializeField] private GameObject boostItem;
    private Dictionary<BoostType, float> unitsBoostDict = new Dictionary<BoostType, float>();
    private Dictionary<BoostType, float> enemiesBoostDict = new Dictionary<BoostType, float>();
    private List<GameObject> boostItemList = new List<GameObject>();
    private float boostLimit = -99f;

    [SerializeField] private Image nextRune;
    [SerializeField] private TMP_Text runeTimer;
    [SerializeField] private TooltipTrigger runeTip;
    [SerializeField] private CanvasGroup runeCanvas;

    public void Init(BattleUIManager manager)
    {
        battleUIManager = manager;
        objectsPool = GlobalStorage.instance.objectsPoolManager;
        boostManager = GlobalStorage.instance.boostManager;
    }

    public void FillPlayerBoost()
    {
        foreach(var item in boostItemList)
            item.SetActive(false);

        boostItemList.Clear();
        unitsBoostDict.Clear();
        enemiesBoostDict.Clear();

        Dictionary<BoostType, List<Boost>> boosts = boostManager.GetBoostDict();
        foreach(var item in boosts)
        {
            List<Boost> tempList = item.Value;

            foreach(var boost in tempList)
            {
                if(boost.effect == BoostEffect.PlayerBattle)
                {
                    if(unitsBoostDict.ContainsKey(item.Key))
                        unitsBoostDict[item.Key] += boost.value;
                    else
                        unitsBoostDict[item.Key] = boost.value;

                    if(unitsBoostDict[item.Key] < boostLimit) unitsBoostDict[item.Key] = boostLimit;
                }

                if(boost.effect == BoostEffect.EnemiesBattle)
                {
                    if(enemiesBoostDict.ContainsKey(item.Key))
                        enemiesBoostDict[item.Key] += boost.value;
                    else
                        enemiesBoostDict[item.Key] = boost.value;

                    if(enemiesBoostDict[item.Key] < boostLimit) enemiesBoostDict[item.Key] = boostLimit;
                }
            }
        }

        foreach(var boost in unitsBoostDict)
        {
            if(boost.Value != 0f)
                CreateEffect(unitsBoostWrapper, EffectType.Rune, boost.Key, boost.Value, true);
        }

        foreach(var boost in enemiesBoostDict)
        {
            if(boost.Value != 0f)
                CreateEffect(enemiesBoostWrapper, EffectType.Enemy, boost.Key, boost.Value, true);
        }
    }

    private void UpgradeBoostes(BoostType boost, float boostValue)
    {
        FillPlayerBoost();
    }

    private void ShowBoostEffect(BoostSender sender, BoostType type, float value)
    {
        EffectType effectType = EffectType.Rune;

        if(sender == BoostSender.EnemySystem) effectType = EffectType.Enemy;
        if(sender == BoostSender.Spell) effectType = EffectType.Spell;

        CreateEffect(fleshBoostWrapper, effectType, type, value, false);
    }

    private void CreateEffect(Transform parent, EffectType effectType, BoostType type, float boost, bool constMode = true)
    {
        GameObject boostItemUI = objectsPool.GetObject(ObjectPool.BattleEffect);
        boostItemUI.transform.SetParent(parent, false);
        boostItemUI.SetActive(true);
        if(parent != fleshBoostWrapper) boostItemList.Add(boostItemUI);

        RunesType runeType = EnumConverter.instance.BoostTypeToRune(type);
        float value = boost;
        BoostInBattleUI item = boostItemUI.GetComponent<BoostInBattleUI>();
        item.Init(runeType, value, constMode, effectType);
    }

    public void SetRuneTimer(Sprite pict, float count, string tip)
    {
        nextRune.sprite = pict;
        runeTimer.text = count.ToString();
        runeTip.content = "Next enemies' boost: " + tip;
        Fading.instance.Fade(true, runeCanvas);
    }

    public void ClearRuneTimer()
    {
        Fading.instance.Fade(false, runeCanvas);
        runeTimer.text = "";
        runeTip.content = "";
    }

    public void UpdateRunetimer(float count)
    {
        runeTimer.text = count.ToString();
    }

    private void OnEnable()
    {
        EventManager.SetBattleBoost += UpgradeBoostes;
        EventManager.ShowBoostEffect += ShowBoostEffect;
    }

    private void OnDisable()
    {
        EventManager.SetBattleBoost -= UpgradeBoostes;
        EventManager.ShowBoostEffect -= ShowBoostEffect;
    }
}
