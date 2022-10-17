using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BonusTipUIManager : MonoBehaviour
{
    public static BonusTipUIManager instance;
    private ObjectsPoolManager poolManager;

    [Serializable]
    public class SpriteAssosiating
    {
        public PlayersStats stat;
        public Sprite sprite;
    }

    public List<SpriteAssosiating> objectsList;

    private int currentHeigth = 0;

    private void Start()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);

        instance.poolManager = GlobalStorage.instance.objectsPoolManager;
    }

    public static void ShowVisualEffect( PlayersStats stat, float quantity, string text = "")     
    {
        Sprite findedSprite = null;

        foreach(var icon in instance.objectsList)
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

        rewardText.transform.position = GlobalStorage.instance.globalPlayer.gameObject.transform.position;
        rewardText.SetActive(true);
        rewardText.GetComponent<BonusTip>().Show(instance.currentHeigth, sprite, quantity, text);
        instance.currentHeigth++;
    }

    public static void HideVisualEffect()
    {
        instance.currentHeigth--;
    }
}
