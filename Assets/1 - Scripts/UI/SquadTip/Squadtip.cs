using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;
using System;

public class Squadtip : MonoBehaviour
{
    private CanvasGroup canvas;

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameItem;
    [SerializeField] private TMP_Text level;
    [SerializeField] private TMP_Text healthCount;
    [SerializeField] private TMP_Text phAttackCount;
    [SerializeField] private TMP_Text mAttackCount;
    [SerializeField] private TMP_Text phDefenceCount;
    [SerializeField] private TMP_Text mDefenceCount;
    [SerializeField] private TMP_Text killCount;
    [SerializeField] private TMP_Text abilityDescr;

    [SerializeField] private GameObject levelBlock;
    [SerializeField] private GameObject killBlock;
    [SerializeField] private GameObject descriptionBlock;

    private PlayerStats playerStats;
    private float curiosity = 0;
    private string gag = "???";

    [SerializeField] private RectTransform rectTransform;
    private float heigthForUnit = 570f;
    private float heigthForEnemy = 390f;

    public void Init(Unit unit)
    {
        if(canvas == null) canvas = GetComponent<CanvasGroup>();

        FillDataSquad(unit);
        ShowSquadExtra(true);
        //FillData(
        //    unit.unitIcon,
        //    unit.unitName,
        //    unit.health,
        //    unit.physicAttack,
        //    unit.magicAttack,
        //    unit.physicDefence,
        //    unit.magicDefence,
        //    unit.killToNextLevel,
        //    unit.abilityDescription
        //    );

        ResizeWindow(heigthForUnit);
        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Init(EnemySO enemy)
    {
        if(canvas == null) canvas = GetComponent<CanvasGroup>();

        if(playerStats == null) playerStats = GlobalStorage.instance.playerStats;
        curiosity = playerStats.GetCurrentParameter(PlayersStats.Curiosity);

        FillDataEnemy(enemy);
        //if(curiosity < 2)
        //{
        //    FillData(enemy.enemyIcon, enemy.enemyName);
        //}
        //else
        //{
        //    FillData(
        //       enemy.enemyIcon,
        //       enemy.enemyName,
        //       enemy.health,
        //       enemy.physicAttack,
        //       enemy.magicAttack,
        //       enemy.physicDefence,
        //       enemy.magicDefence
        //       );
        //}
        ShowSquadExtra(false);

        ResizeWindow(heigthForEnemy);
        Fading.instance.FadeWhilePause(true, canvas);
    }


    //private void FillData(Sprite pict, string name, float health, float pA, float mA, float pD, float mD, float kill = 0, string desc = "") 
    private void FillDataSquad(Unit unit)
    {
        icon.sprite         = unit.unitIcon;
        nameItem.text       = unit.unitName;
        level.text          = unit.level.ToString();
        healthCount.text    = unit.health.ToString();
        phAttackCount.text  = unit.physicAttack.ToString();
        mAttackCount.text   = unit.magicAttack.ToString();
        phDefenceCount.text = unit.physicDefence.ToString();
        mDefenceCount.text  = unit.magicDefence.ToString();
        killCount.text      = unit.killToNextLevel.ToString();
        abilityDescr.text   = unit.abilityDescription;

        //if(desc != "") abilityDescription.text = desc.ToString();
    }

    private void FillDataEnemy(EnemySO enemy)
    {
        icon.sprite         = enemy.enemyIcon;
        nameItem.text       = enemy.enemyName;
        healthCount.text    = (curiosity < 2) ? gag : enemy.health.ToString();
        phAttackCount.text  = (curiosity < 2) ? gag : enemy.physicAttack.ToString();
        mAttackCount.text   = (curiosity < 2) ? gag : enemy.magicAttack.ToString();
        phDefenceCount.text = (curiosity < 2) ? gag : enemy.physicDefence.ToString();
        mDefenceCount.text  = (curiosity < 2) ? gag : enemy.magicDefence.ToString();
    }
    //private void FillData(Sprite pict, string name)
    //{
    //    icon.sprite = pict;
    //    nameItem.text = name;
    //    healthCount.text = gag;
    //    phAttackCount.text = gag;
    //    mAttackCount.text = gag;
    //    phDefenceCount.text = gag;
    //    mDefenceCount.text = gag;
    //}

    private void ShowSquadExtra(bool mode)
    {
        levelBlock.SetActive(mode);
        killBlock.SetActive(mode);
        descriptionBlock.SetActive(mode);
    }

    private void ResizeWindow(float heigth)
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heigth);
    }
}
