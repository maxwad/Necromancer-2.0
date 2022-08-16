using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BonusManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject health;
    [SerializeField] private GameObject mana;
    [SerializeField] private GameObject gold;
    [SerializeField] private GameObject tempExp;

    [Header("Sprites")]
    [SerializeField] private List<Sprite> healthBigSpriteList;
    [SerializeField] private List<Sprite> healthLargeSpriteList;
    [Space]
    [SerializeField] private List<Sprite> manaBigSpriteList;
    [SerializeField] private List<Sprite> manaLargeSpriteList;
    [Space]
    [SerializeField] private List<Sprite> goldBigSpriteList;
    [SerializeField] private List<Sprite> goldLargeSpriteList;
    [Space]
    [SerializeField] private List<Sprite> tempExpBigSpriteList;
    [SerializeField] private List<Sprite> tempExpLargeSpriteList;

    private float bonusBoost = 0;
    private GameObject currentBonus;
    public List<GameObject> bonusesOnTheMap = new List<GameObject>();

    private bool isEnoughTempExp = false;
    private BonusType alternativeBonusType = BonusType.Gold;

    public void CreateBonus(bool isThisFromBoss, BonusType type, Vector3 position, float value = 0)
    {
        if(isEnoughTempExp == true && type == BonusType.TempExp)
        {
            type = alternativeBonusType;
        }

        switch (type)
        {
            case BonusType.Health:
                currentBonus = health;
                break;

            case BonusType.Mana:
                currentBonus = mana;
                break;

            case BonusType.Gold:
                currentBonus = gold;
                break;

            case BonusType.TempExp:
                currentBonus = tempExp;
                break;

            case BonusType.Other:
                break;

            case BonusType.Nothing:
                return;

            default:
                return;
        }

        GameObject bonus;

        if (currentBonus.GetComponent<BonusController>().isFromPoolObject == true)
        {
            ObjectPool objectPoolType = ObjectPool.BonusExp;
            if(type == BonusType.Gold) objectPoolType = ObjectPool.BonusGold;

            bonus = GlobalStorage.instance.objectsPoolManager.GetObjectFromPool(objectPoolType);
            bonus.transform.position = position;
            bonus.SetActive(true);
        }
        else
        {
            bonus = Instantiate(currentBonus, position, Quaternion.identity);
            bonus.transform.SetParent(GlobalStorage.instance.bonusesContainer.transform);
        }


        if(value != 0) bonus.GetComponent<BonusController>().SetBonusValue(value);
        bonus.GetComponent<BonusController>().BoostBonusValue(bonusBoost, isThisFromBoss);

        bonusesOnTheMap.Add(bonus);
    }

    private void ClearBonusList(bool mode)
    {
        if(mode == true) bonusesOnTheMap.Clear();
    }

    public void BoostBonus(float value)
    {
        bonusBoost = value;
    }

    public List<Sprite> GetNewSprites(BonusType type, int size)
    {
        switch(type)
        {
            case BonusType.Health:
                if(size == 1) return healthBigSpriteList;
                if(size == 2) return healthLargeSpriteList;
                break;

            case BonusType.Mana:
                if(size == 1) return manaBigSpriteList;
                if(size == 2) return manaLargeSpriteList;
                break;

            case BonusType.Gold:
                if(size == 1) return goldBigSpriteList;
                if(size == 2) return goldLargeSpriteList;
                break; 

            case BonusType.TempExp:
                if(size == 1) return tempExpBigSpriteList;
                if(size == 2) return tempExpLargeSpriteList;
                break;

            default:
                break;
        }

        return null;
    }

    private void ExpEnough(bool mode)
    {
        isEnoughTempExp = mode;
    }

    private void OnEnable()
    {
        EventManager.ChangePlayer += ClearBonusList;
        EventManager.ExpEnough += ExpEnough;
    }

    private void OnDisable()
    {
        EventManager.ChangePlayer -= ClearBonusList;
        EventManager.ExpEnough -= ExpEnough;
    }
}
