using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;
using System;

public class Squadtip : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameItem;
    [SerializeField] private TMP_Text healthCount;
    [SerializeField] private TMP_Text phAttackCount;
    [SerializeField] private TMP_Text mAttackCount;
    [SerializeField] private TMP_Text phDefenceCount;
    [SerializeField] private TMP_Text mDefenceCount;
    [SerializeField] private TMP_Text abilityDescription;

    [SerializeField] private GameObject descriptionBlock;

    private PlayerStats playerStats;
    private float curiosity = 0;
    private string gag = "???";

    private float offset = 20f;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Init(Unit unit)
    {
        ShowAbility(true);
        string ability = WeaponsDictionary.instance.GetAbilityDescription(unit.unitAbility, unit.level);

        FillData(
            unit.unitIcon,
            unit.unitName,
            unit.health,
            unit.physicAttack,
            unit.magicAttack,
            unit.physicDefence,
            unit.magicDefence,
            ability
            );
    }

    public void Init(Enemy enemy)
    {
        ShowAbility(false);

        if(playerStats == null) playerStats = GlobalStorage.instance.playerStats;
        curiosity = playerStats.GetCurrentParameter(PlayersStats.Curiosity);
        if(curiosity < 2)
        {
            FillData(enemy.enemyIcon, enemy.enemyName);
        }
        else
        {
            FillData(
               enemy.enemyIcon,
               enemy.enemyName,
               enemy.health,
               enemy.physicAttack,
               enemy.magicAttack,
               enemy.physicDefence,
               enemy.magicDefence
               );
        }        
    }

    private void FillData(Sprite pict, string name, float health, float pA, float mA, float pD, float mD, string desc = "") 
    {
        icon.sprite = pict;
        nameItem.text = name;
        healthCount.text = health.ToString();
        phAttackCount.text = pA.ToString();
        mAttackCount.text = mA.ToString();
        phDefenceCount.text = pD.ToString();
        mDefenceCount.text = mD.ToString();

        if(desc != "") abilityDescription.text = desc.ToString();
    }

    private void FillData(Sprite pict, string name)
    {
        icon.sprite = pict;
        nameItem.text = name;
        healthCount.text = gag;
        phAttackCount.text = gag;
        mAttackCount.text = gag;
        phDefenceCount.text = gag;
        mDefenceCount.text = gag;
    }

    private void ShowAbility(bool mode)
    {
        descriptionBlock.SetActive(mode);
    }

    private void LateUpdate()
    {
        CorrectSquadtipPosition();
    }

    private void CorrectSquadtipPosition()
    {
        Vector2 position = Input.mousePosition;
        float pivotX = 0;
        float pivotY = 1;
        float offsetX = offset;
        float offsetY = 0;

        if(Screen.width - position.x < rectTransform.rect.width)
        {
            pivotX = 1f;
            offsetX = -offsetX;
        }

        if(position.y - rectTransform.rect.height < 0)
        {
            pivotY = 0f;
        }

        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position + new Vector2(offsetX, offsetY);
    }
}
