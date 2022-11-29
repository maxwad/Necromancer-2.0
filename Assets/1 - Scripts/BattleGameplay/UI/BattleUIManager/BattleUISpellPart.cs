using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NameManager;

public class BattleUISpellPart : MonoBehaviour
{
    private BattleUIManager battleUIManager;
    private ObjectsPoolManager poolManager;
    private BoostManager boostManager;

    [Header("Spells")]
    [SerializeField] private Button buttonSpell;
    private List<SpellSO> currentSpells = new List<SpellSO>();
    [SerializeField] private GameObject spellButtonContainer;
    private List<Button> currentSpellsButtons = new List<Button>();

    [SerializeField] private GameObject spellEffectsContainer;
    private Dictionary<SpellSO, SpellBattleUI> currentActiveSpells = new Dictionary<SpellSO, SpellBattleUI>();

    private int countOfActiveSpells = 6;
    private int currentSpellIndex = -1;

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

        if(currentSpellIndex != -1)
            currentSpellsButtons[currentSpellIndex].GetComponent<SpellButtonController>().ActivateSpell();
    }

    public void FillSpells(int numberOfSpell)
    {
        if(poolManager == null) 
        {
            poolManager = GlobalStorage.instance.objectsPoolManager;
            boostManager = GlobalStorage.instance.boostManager;
        }

        currentActiveSpells.Clear();

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
                button.GetComponent<SpellButtonController>().InitializeButton(this, currentSpells[i], slotNumber);
                currentSpellsButtons.Add(button);
                button.transform.SetParent(spellButtonContainer.transform, false);

            }
        }
        else
        {
            for(int i = 0; i < countOfActiveSpells; i++)
            {
                if(i == numberOfSpell)
                {
                    Button button = Instantiate(buttonSpell);
                    button.GetComponent<SpellButtonController>().InitializeButton(this, currentSpells[i]);
                    currentSpellsButtons.Add(button);
                    button.transform.SetParent(spellButtonContainer.transform, false);

                    break;
                }
            }
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
        GameObject effectGO = poolManager.GetObject(ObjectPool.SpellEffect);
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

            List<BoostType> boosts = EnumConverter.instance.SpellToBoost(spell.spell);
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
