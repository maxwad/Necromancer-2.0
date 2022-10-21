using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BattleBoostManager : MonoBehaviour
{
    //common effects (0..1)

    //speed
    public float bonusSpeed_FromCastle       = 0;
    public float bonusSpeed_FromMapBuildings = 0;
    public float bonusSpeed_FromBoostSystem  = 0;
    public float bonusSpeed_FromPlayer       = 0;
    public float bonusSpeed_FromWeekEffect   = 0;
    public float bonusSpeed_FromMapIncident  = 0;
    public float bonusSpeed_FromEnemyBoss    = 0;
    public float bonusSpeed_FromEnemySpell   = 0;
    public float bonusSpeed_FromRandom       = 0;

    public float bonusSpeed_Total = 0;

    //private effects

    //methods
    public Unit AddBonusStatsToUnit(Unit unit)
    {
        //some code
        return unit;
    }

    public Enemy AddBonusStatsToEnemy(Enemy enemy)
    {
        //some code
        return enemy;
    }

    private void SetBoost(bool boostAll, bool addBoost, BoostSender sender, UnitStats stat, float value, UnitsTypes types = UnitsTypes.Militias)
    {
        Debug.Log("We set " + addBoost + " to " + stat + " boost = " + value);
    }

    private void OnEnable()
    {
        EventManager.BoostUnitStat += SetBoost;
    }

    private void OnDisable()
    {
        EventManager.BoostUnitStat -= SetBoost;
    }
}
