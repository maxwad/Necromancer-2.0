using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BonusController : MonoBehaviour
{
    private BoostManager boostManager;

    private BonusSO bonusSO;
    public BonusType bonusType;
    [HideInInspector] public float baseValue;
    public float value;
    private bool isSpecialBonus = false;

    private GameObject player;
    private bool isActivate = false;
    private float speed = 30f;
    private float inertion = -32f;
    private float currentInertion;

    private BonusManager bonusManager;
    private List<Sprite> currentSpriteList = new List<Sprite>();
    private SimpleAnimator animator;


    private void Awake()
    {
        currentInertion = inertion;
        value = baseValue;
    }

    public void Init(bool specialMode, BonusSO bonus, float forceValue = 0)
    {
        if(bonus == null) return;
        if(boostManager == null) boostManager = GlobalStorage.instance.boostManager;

        ResetBonusValue();

        bonusSO = bonus;
        bonusType = bonus.bonusType;
        isSpecialBonus = specialMode;
        baseValue = (forceValue == 0) ? bonus.value : forceValue;

        if(bonusType == BonusType.TempExp)
            value = baseValue + baseValue * boostManager.GetBoost(BoostType.Exp);
        else
            value = baseValue + baseValue * boostManager.GetBoost(BoostType.BonusAmount);

        gameObject.SetActive(true);

        CheckSizeOfBonus();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TagManager.T_PLAYER))
        {
            EventManager.OnBonusPickedUpEvent(bonusType, value);
            DestroyMe();
        }
    }

    public void ActivatateBonus()
    {
        if(isActivate == false)
        {
            isActivate = true;
            animator.StopAnimation(true);
            StartCoroutine("ToThePlayer");            
        }        
    }

    private IEnumerator ToThePlayer()
    {
        player = GlobalStorage.instance.hero.gameObject;
        while(player.activeInHierarchy == true && isActivate == true)
        {
            float step = (speed + currentInertion) * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
            if(currentInertion < 0) currentInertion += 0.1f;

            yield return new WaitForSeconds(0.001f);
        }
    }

    public void DestroyMe()
    {
        isActivate = false;
        bonusManager.bonusesOnTheMap.Remove(gameObject);
        gameObject.SetActive(false);       
    }

    public void BoostBonusValue(float boost)
    {
        value = baseValue + (baseValue * boost);
        if(isSpecialBonus == false) CheckSizeOfBonus();
    }

    private void ResetBonusValue()
    {
        currentInertion = inertion;
        isSpecialBonus = false;

        if(bonusSO != null) 
        {
            baseValue = bonusSO.value;
            value = baseValue;
        }
    }

    private void CheckSizeOfBonus()
    {
        int size = 0;

        if(value > baseValue * 2) size = 1;

        if(value > baseValue * 5) size = 2;

        if(isSpecialBonus == true) size = 2;

        if(size == 0) currentSpriteList = bonusSO.spritesSmall;
        if(size == 1) currentSpriteList = bonusSO.spritesMiddle;
        if(size == 2) currentSpriteList = bonusSO.spritesLarge;

        animator.ChangeAnimation(currentSpriteList);
    }

    private void UpgradeParameters(BoostType boost, float boostValue)
    {
        if(boost == BoostType.BonusAmount) 
        {
            value = baseValue + baseValue * boostValue;
            CheckSizeOfBonus();        
        }
    }

    private void OnEnable()
    {
        EventManager.Victory += ActivatateBonus;
        EventManager.EndOfBattle += DestroyMe;
        EventManager.SetBattleBoost += UpgradeParameters;

        if(animator == null)
        {
            bonusManager = GlobalStorage.instance.bonusManager;
            animator = GetComponent<SimpleAnimator>();
        }
    }

    private void OnDisable()
    {
        EventManager.Victory -= ActivatateBonus;
        EventManager.EndOfBattle -= DestroyMe;
        EventManager.SetBattleBoost -= UpgradeParameters;
    }
}
