using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;

public class Squadtip : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvas;

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameItem;
    [SerializeField] private TMP_Text healthCount;
    [SerializeField] private TMP_Text phAttackCount;
    [SerializeField] private TMP_Text mAttackCount;
    [SerializeField] private TMP_Text phDefenceCount;
    [SerializeField] private TMP_Text mDefenceCount;
    [SerializeField] private TMP_Text killCount;
    [SerializeField] private TMP_Text abilityDescription;

    [SerializeField] private GameObject killBlock;
    [SerializeField] private GameObject descriptionBlock;

    private PlayerStats playerStats;
    private float curiosity = 0;
    private string gag = "???";

    [SerializeField] private RectTransform rectTransform;
    private float heigthForUnit = 530f;
    private float heigthForEnemy = 390f;

    public void Init(Unit unit)
    {
        if(canvas == null) canvas = GetComponent<CanvasGroup>();
        ShowAbility(true);

        FillData(
            unit.unitIcon,
            unit.unitName,
            unit.health,
            unit.physicAttack,
            unit.magicAttack,
            unit.physicDefence,
            unit.magicDefence,
            unit.killToNextLevel,
            unit.abilityDescription
            );

        ResizeWindow(heigthForUnit);
        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Init(EnemySO enemy)
    {
        if(canvas == null) canvas = GetComponent<CanvasGroup>();
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

        ResizeWindow(heigthForEnemy);
        Fading.instance.FadeWhilePause(true, canvas);
    }

    private void FillData(Sprite pict, string name, float health, float pA, float mA, float pD, float mD, float kill = 0, string desc = "") 
    {
        icon.sprite = pict;
        nameItem.text = name;
        healthCount.text = health.ToString();
        phAttackCount.text = pA.ToString();
        mAttackCount.text = mA.ToString();
        phDefenceCount.text = pD.ToString();
        killCount.text = kill.ToString();
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
        killBlock.SetActive(mode);
        descriptionBlock.SetActive(mode);
    }

    private void ResizeWindow(float heigth)
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heigth);
    }
}
