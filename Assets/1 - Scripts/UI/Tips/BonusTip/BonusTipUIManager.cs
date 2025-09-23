using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static Enums;

public class BonusTipUIManager : MonoBehaviour
{
    public static BonusTipUIManager instance;
    private ObjectsPoolManager poolManager;
    private GMPlayerMovement globalPlayer;

    public List<GMSpriteAssosiating> objectsGMList;
    public List<BattleSpriteAssosiating> objectsBattleList;

    private int currentHeigth = 0;

    [Inject]
    public void Construct(ObjectsPoolManager poolManager, GMPlayerMovement globalPlayer)
    {
        this.poolManager = poolManager;
        this.globalPlayer = globalPlayer;
    }

    private void Start()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static void ShowVisualEffect(PlayersStats stat, float quantity, string text = "")     
    {
        Sprite findedSprite = null;

        foreach(var icon in instance.objectsGMList)
        {
            if(icon.stat == stat)
            {
                findedSprite = icon.sprite;
                break;
            }
        }

        ShowVisualEffect(findedSprite, quantity, text);
    }


    public static void ShowVisualEffect(Sprite sprite, float quantity, string text = "")
    {
        GameObject rewardText = instance.poolManager.GetObject(ObjectPool.BonusText);

        rewardText.transform.position = instance.globalPlayer.gameObject.transform.position;
        rewardText.SetActive(true);
        rewardText.GetComponent<BonusTip>().Show(instance.currentHeigth, sprite, quantity, text);
        instance.currentHeigth++;
    }

    public static void ShowVisualEffect(Vector3 position, VisualEffects effect, float quantity = 0, string text = "", string mark = "")
    {
        Sprite findedSprite = null;

        foreach(var icon in instance.objectsBattleList)
        {
            if(icon.effect == effect)
            {
                findedSprite = icon.sprite;
                break;
            }
        }

        GameObject rewardText = instance.poolManager.GetObject(ObjectPool.BonusText);

        rewardText.transform.position = position;
        rewardText.SetActive(true);
        rewardText.GetComponent<BonusTip>().Show(instance.currentHeigth, findedSprite, quantity, text, mark);
        instance.currentHeigth++;
    }

    public static void ShowVisualEffectForEnemy(Sprite sprite, float quantity, Vector3 position, string text = "")
    {
        GameObject rewardText = instance.poolManager.GetObject(ObjectPool.BonusText);

        rewardText.transform.position = position;
        rewardText.SetActive(true);
        rewardText.GetComponent<BonusTip>().Show(instance.currentHeigth, sprite, quantity, text);
        instance.currentHeigth++;
    }

    public static void ShowVisualEffectInBattle(Vector3 position, VisualEffects effect, float quantity = 0, string text = "", string mark = "")
    {
        if(GlobalStorage.instance.IsGlobalMode() == true) return;

        Sprite findedSprite = null;

        foreach(var icon in instance.objectsBattleList)
        {
            if(icon.effect == effect)
            {
                findedSprite = icon.sprite;
                break;
            }
        }

        GameObject rewardText = instance.poolManager.GetObject(ObjectPool.BonusText);

        rewardText.transform.position = position;
        rewardText.SetActive(true);
        rewardText.GetComponent<BonusTip>().Show(instance.currentHeigth, findedSprite, quantity, text, mark);
    }

    public static void HideVisualEffect()
    {
        instance.currentHeigth--;
    }
}
