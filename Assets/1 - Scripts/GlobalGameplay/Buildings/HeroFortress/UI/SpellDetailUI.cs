using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class SpellDetailUI : MonoBehaviour
{
    [Header("Details")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text spellName;
    [SerializeField] private TMP_Text level;
    [SerializeField] private TMP_Text manaCost;
    [SerializeField] private TMP_Text duration;
    [SerializeField] private TMP_Text reloading;
    [SerializeField] private TMP_Text description;

    [Header("Extra Elements")]
    [SerializeField] private GameObject upgradeLabel;
    [SerializeField] private GameObject confirmBlock;
    [SerializeField] private GameObject createBlock;


    private SpellWorkroom workroom;
    private SpellSO currentSpell;
    private bool isCreating = false;

    public void FillData(SpellWorkroom room, SpellSO spell, bool isFirstSpell, bool createMode)
    {
        if(workroom == null) workroom = room;

        isCreating = createMode;

        currentSpell = spell;

        spellName.text = spell.spellName;
        icon.sprite = spell.icon;
        level.text = spell.level.ToString();

        manaCost.text = spell.manaCost.ToString();
        duration.text = (spell.actionTime != 0) ? spell.actionTime.ToString() : "-";
        reloading.text = spell.reloading.ToString();

        description.text = spell.description
            .Replace("$V", spell.value.ToString())
            .Replace("$R", spell.radius.ToString())
            .Replace("$T", spell.actionTime.ToString());

        //foreach(var item in spell.cost)
        //{

        //}
        Refactoring(isFirstSpell, createMode);
    }

    private void Refactoring(bool isFirstSpell, bool createMode)
    {
        spellName.gameObject.SetActive(false);
        upgradeLabel.SetActive(false);
        confirmBlock.SetActive(false);
        createBlock.SetActive(false);

        if(isFirstSpell == true)
        {
            spellName.gameObject.SetActive(true);
            if(createMode == true)
            {                
                createBlock.SetActive(true);
            }
        }
        else
        {
            upgradeLabel.SetActive(true);
            createBlock.SetActive(true);
        }
    }

    public void TryToAction()
    {
        confirmBlock.SetActive(true);
    }

    public void Action()
    {
        if(isCreating == true)
            Create();
        else
            Upgrade();


        confirmBlock.SetActive(true);
    }

    private void Create()
    {

    }

    private void Upgrade()
    {

    }
}
