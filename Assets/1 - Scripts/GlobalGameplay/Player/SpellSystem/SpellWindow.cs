using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static Enums;

public class SpellWindow : MonoBehaviour
{
    private SpellManager spellManager;
    private PlayerStats playerStats;

    [SerializeField] private List<BattleSpellItemUI> storageSpellsList;
    [SerializeField] private List<BattleSpellItemUI> battleSpellsList;

    private List<SpellSO> spellsForStorage;
    private List<SpellSO> spellsForBattle;

    private float spellsAmount;

    [Inject]
    public void Construct(SpellManager spellManager, PlayerStats playerStats)
    {
        this.spellManager = spellManager;
        this.playerStats = playerStats;
    }

    public void Init()
    {
        ResetStorage();
        ResetSpellsList();

        FillSpells();
    }

    private void ResetStorage()
    {
        foreach(var spellItem in storageSpellsList)
        {
            spellItem.gameObject.SetActive(false);
        }
    }

    private void ResetSpellsList()
    {
        spellsAmount = playerStats.GetCurrentParameter(PlayersStats.Spell);

        for(int i = 0; i < battleSpellsList.Count; i++)
        {            
            battleSpellsList[i].SetIndexes(i + 1, i < spellsAmount);
        }
    }

    private void FillSpells()
    {
        spellsForStorage = spellManager.GetSpellsForStorage();

        for(int i = 0; i < spellsForStorage.Count; i++)
        {
            storageSpellsList[i].gameObject.SetActive(true);
            storageSpellsList[i].Init(this, spellsForStorage[i]);
        }

        spellsForBattle = spellManager.GetSpellsForBattle();

        for(int i = 0; i < spellsForBattle.Count; i++)
        {
            battleSpellsList[i].gameObject.SetActive(true);
            battleSpellsList[i].Init(this, spellsForBattle[i]);
        }
    }

    public void SwitchSpells(Spells spell, bool isFromStorage)
    {
        bool result = spellManager.SwitchSpells(spell, isFromStorage);

        if(result == true) Init();
    }
}