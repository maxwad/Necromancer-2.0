using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BonusController : MonoBehaviour
{
    public BonusType bonusType;
    [SerializeField] private float baseValue;
    [SerializeField] private float originalBaseValue;
    public float value;
    public bool isFromPoolObject = false;
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
        originalBaseValue = baseValue;

        bonusManager = GlobalStorage.instance.bonusManager;
        animator = GetComponent<SimpleAnimator>();
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
        ResetBonusValue();

        if (isFromPoolObject == true)
        {
            currentInertion = inertion;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void BoostBonusValue(float boost, bool isThisFromBoss = false)
    {
        value = baseValue + (baseValue * boost);
        if(isSpecialBonus == false) CheckSizeOfBonus(isThisFromBoss);
    }

    public void SetBonusValue(float newValue)
    {
        baseValue = newValue;
    }

    private void ResetBonusValue()
    {
        baseValue = originalBaseValue;
        value = originalBaseValue;
        isSpecialBonus = false;
    }

    private void CheckSizeOfBonus(bool isThisFromBoss)
    {
        int size = 0;

        if(value > baseValue) size = 1;

        if(value > baseValue * 3) size = 2;

        if(isThisFromBoss == true)
        {
            isSpecialBonus = true;
            size = 2;
        }

        if(size != 0)
        {
            currentSpriteList = bonusManager.GetNewSprites(bonusType, size);
            animator.ChangeAnimation(currentSpriteList);
        }
        else
        {
            animator.ResetAnimation();
        }
    }

    private void OnEnable()
    {
        EventManager.Victory += ActivatateBonus;
        EventManager.EndOfBattle += DestroyMe;
    }

    private void OnDisable()
    {
        EventManager.Victory -= ActivatateBonus;
        EventManager.EndOfBattle -= DestroyMe;
    }
}
