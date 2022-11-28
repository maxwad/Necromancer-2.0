using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class EnemyEffector : MonoBehaviour
{
    private RunesManager runesManager;
    private BattleBoostManager boostManager;
    private BattleUIManager battleUI;

    private List<RunesType> availableRuneTypes = new List<RunesType>();
    private List<RuneSO> runes = new List<RuneSO>();
    private Dictionary<RunesType, float> runeBounds = new Dictionary<RunesType, float>();
    private RuneSO currentRune;
    private float runeBound = 8;

    private ArmyStrength currentArmyStrenght;
    private float timeScaler = 20;
    private float timePortionDefault = 140; //180
    private float timePortion;
    private float timerCount;
    //private int countOfStrengthening;
    //private int maxOfStrengthening = 14;
    private float nextRuneDelay = 1f;
    private float timerStep = 1f;
    private Coroutine nextRuneCoroutine;
    private Coroutine timerCoroutine;

    private void Start()
    {
        boostManager = GlobalStorage.instance.boostManager;
        runesManager = GlobalStorage.instance.runesManager;
        battleUI = GlobalStorage.instance.battleIUManager;
    }

    public void Init(ArmyStrength strength)
    {
        currentArmyStrenght = strength;
        //countOfStrengthening = 0;
        timePortion = timePortionDefault - timeScaler * ((int)currentArmyStrenght - 1);
        timerCount = timePortion;

        GetRunes();

        nextRuneCoroutine = StartCoroutine(NextRune());
    }


    private void GetRunes()
    {
        runes.Clear();
        List<RuneSO> tempList = runesManager.runesStorage.GetEnemySystemRunes();

        for(int i = 0; i < tempList.Count; i++)
        {
            if(tempList[i].level <= (int)currentArmyStrenght)
            {
                runes.Add(tempList[i]);
            }
        }
            

        runeBounds.Clear();

        for(int i = 0; i < runes.Count; i++)
        {
            if(runeBounds.ContainsKey(runes[i].rune) == true)
                continue;
            else
                runeBounds[runes[i].rune] = 0;
        }

        foreach(var type in runeBounds)
            availableRuneTypes.Add(type.Key);
    }

    private RuneSO GetRandomRune()
    {
        RuneSO rune;

        if(runes.Count == 0) return null;

        int randomIndex = Random.Range(0, runes.Count);

        rune = runes[randomIndex];

        runeBounds[rune.rune] += rune.level;
        if(runeBounds[rune.rune] >= runeBound)
        {
            int count = runes.Count;
            while(count > 0)
            {
                count--;
                if(runes[count].rune == rune.rune)
                    runes.Remove(runes[count]);
            }
        }

        return rune;        
    }

    private void SendBeforeRune(RuneSO rune)
    {
        if(rune == null) return;

        string tip = rune.positiveDescription.Replace("$", rune.value.ToString());
        battleUI.boostPart.SetRuneTimer(rune.activeIcon, timePortion, tip);

        timerCoroutine = StartCoroutine(Timer());
    }

    private void SendAfterRune(RuneSO rune)
    {
        battleUI.boostPart.ClearRuneTimer();

        BoostType type = EnumConverter.instance.RuneToBoostType(rune.rune);
        float value = (rune.isInvertedRune == true) ? -rune.value : rune.value;
        boostManager.SetBoost(type, BoostSender.EnemySystem, BoostEffect.EnemiesBattle, value);

        nextRuneCoroutine = StartCoroutine(NextRune());
    }

    private IEnumerator NextRune()
    {
        yield return new WaitForSeconds(nextRuneDelay);

        currentRune = GetRandomRune();
        timerCount = timePortion;
        SendBeforeRune(currentRune);
        //countOfStrengthening++;
    }

    private IEnumerator Timer()
    {
        while(timerCount > 0)
        {
            battleUI.boostPart.UpdateRunetimer(timerCount);
            timerCount--;
            yield return new WaitForSeconds(timerStep);
        }

        SendAfterRune(currentRune);
    }

    public void StopEffector()
    {
        battleUI.boostPart.ClearRuneTimer();
        if(nextRuneCoroutine != null) StopCoroutine(nextRuneCoroutine);

        if(timerCoroutine != null) StopCoroutine(timerCoroutine);
    }

    private void Victory()
    {
        StopEffector();
    }

    private void OnEnable()
    {
        EventManager.Victory += Victory;
    }

    private void OnDisable()
    {
        EventManager.Victory -= Victory;
    }
}
