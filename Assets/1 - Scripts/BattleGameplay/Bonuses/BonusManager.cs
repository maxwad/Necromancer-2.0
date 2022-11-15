using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BonusManager : MonoBehaviour
{
    [Header("Prefabs")]
    private ObjectsPoolManager poolManager;
    [SerializeField] private List<BonusSO> bonuses;
    public List<GameObject> bonusesOnTheMap = new List<GameObject>();

    private bool isEnoughTempExp = false;
    private BonusType alternativeBonusType = BonusType.Gold;

    private void Start()
    {
        poolManager = GlobalStorage.instance.objectsPoolManager;        
    }

    public void CreateBonus(bool isThisFromBoss, BonusType type, Vector3 position, float value = 0)
    {
        if(isEnoughTempExp == true && type == BonusType.TempExp)
        {
            type = alternativeBonusType;
        }
        
        GameObject bonus;

        BonusSO bonusSO = null;

        foreach(var bonusItem in bonuses)
        {
            if(bonusItem.bonusType == type)
            {
                bonusSO = bonusItem;
                break;
            }
        }

        bonus = poolManager.GetObject(ObjectPool.BattleBonus);
        bonus.transform.position = position;
        bonus.transform.SetParent(GlobalStorage.instance.bonusesContainer.transform);
        bonus.GetComponent<BonusController>().Init(isThisFromBoss, bonusSO, value);        

        bonusesOnTheMap.Add(bonus);
    }

    private void ClearBonusList(bool mode)
    {
        //if(mode == true) bonusesOnTheMap.Clear();
    }

    private void ExpEnough(bool mode)
    {
        isEnoughTempExp = mode;
    }

    private void OnEnable()
    {
        EventManager.SwitchPlayer += ClearBonusList;
        EventManager.ExpEnough += ExpEnough;
    }

    private void OnDisable()
    {
        EventManager.SwitchPlayer -= ClearBonusList;
        EventManager.ExpEnough -= ExpEnough;
    }
}
