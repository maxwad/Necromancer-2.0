using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Enums;

public class BattleUISpellPart : MonoBehaviour
{
    private BattleUIManager battleUIManager;
    private ObjectsPoolManager objectsPool;
    private BoostManager boostManager;
    private PlayerStats playerStats;
    private SpellManager spellManager;

    [Header("Spells")]
    [SerializeField] private List<SpellButtonController> allSpellButtons;
    private List<SpellSO> currentSpells = new List<SpellSO>();

    [SerializeField] private GameObject spellEffectsContainer;
    private Dictionary<SpellSO, SpellBattleUI> currentActiveSpells = new Dictionary<SpellSO, SpellBattleUI>();

    private int countOfActiveSpells = 6;
    private int currentSpellIndex = -1;
    private int realNumberOfSpells = -1;

    [Inject]
    public void Construct(
        BoostManager boostManager,
        ObjectsPoolManager objectsPool,
        BattleUIManager battleUIManager,
        PlayerStats playerStats,
        SpellManager spellManager
        )
    {
        this.boostManager = boostManager;
        this.objectsPool = objectsPool;
        this.battleUIManager = battleUIManager;
        this.playerStats = playerStats;
        this.spellManager = spellManager;
    }

    private void Start()
    {
        for(int i = 0; i < allSpellButtons.Count; i++)
        {
            allSpellButtons[i].PreInit(this, i + 1);
        }
    }

    public void Init(BattleUIManager manager)
    {
        battleUIManager = manager;
    }

    private void Update()
    {   
        if(GlobalStorage.instance.isGlobalMode == false && battleUIManager.isBattleOver == false) 
            Spelling();
    }

    private void Spelling()
    {
        currentSpellIndex = -1;

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
        }

        if(currentSpellIndex != -1 && currentSpellIndex < realNumberOfSpells)
            allSpellButtons[currentSpellIndex].ActivateSpell();

            //currentSpellsButtons[currentSpellIndex].GetComponent<SpellButtonController>().ActivateSpell();
    }

    public void FillSpells()
    {
        currentActiveSpells.Clear();

        currentSpells = spellManager.GetSpellsForBattle();
        if(currentSpells.Count == 0) return;

        countOfActiveSpells = (int)playerStats.GetCurrentParameter(PlayersStats.Spell);

        int counter = (currentSpells.Count < countOfActiveSpells) ? currentSpells.Count : countOfActiveSpells;
        realNumberOfSpells = counter;

        for(int i = 0; i < counter; i++)
        {
            if(i >= allSpellButtons.Count) break;

            allSpellButtons[i].InitializeButton(currentSpells[i]);
        }
    }
        
    public bool CheckBattleOver()
    {
        return battleUIManager.isBattleOver;
    }

    public bool CheckSpell(SpellSO spell)
    {
        return currentActiveSpells.ContainsKey(spell);
    }

    public void AddUISpellEffect(SpellSO spell)
    {
        GameObject effectGO = objectsPool.GetObject(ObjectPool.SpellEffect);
        effectGO.transform.SetParent(spellEffectsContainer.transform, false);
        effectGO.SetActive(true);
        SpellBattleUI effectUI = effectGO.GetComponent<SpellBattleUI>();
        effectUI.Init(this, spell);
        currentActiveSpells.Add(spell, effectUI);
    }

    public void DeleteUISpellEffect(SpellSO spell)
    {
        if(currentActiveSpells.ContainsKey(spell) == true)
        {
            currentActiveSpells.Remove(spell);

            List<BoostType> boosts = TypesConverter.SpellToBoost(spell.spell);
            if(boosts.Count == 0)
            {
                Debug.Log("There is smth wrong!");
            }
            else
            {
                foreach(var item in boosts)
                {
                    boostManager.DeleteBoost(item, BoostSender.Spell, spell.value);
                }
            }
        }
    }

    public void ProlongSpell(SpellSO spell)
    {
        currentActiveSpells[spell].AddActionTime(spell.actionTime);
    }
}
