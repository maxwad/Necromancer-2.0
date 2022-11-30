using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class OpeningBuildingWindow : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvas;

    [SerializeField] private TMP_Text caption;
    [SerializeField] private GameObject status;
    [SerializeField] private List<GameObject> levelsList;

    //[SerializeField] private Image levelIcon;
    //[SerializeField] private Color lockColor;
    //[SerializeField] private Color unlockColor;
    //[SerializeField] private Sprite lockIcon;
    //[SerializeField] private Sprite unlockIcon;

    [SerializeField] private List<GameObject> activeScreens;


    public void Open(FBuilding building)
    {
        foreach(var screen in activeScreens)
            screen.SetActive(false);

        caption.text = building.building.ToString();

        if(building.isPassiveEffect == true)
        {
            status.SetActive(true);
        }
        else
        {
            status.SetActive(false);
            //Enabling active component
        }

        List<RuneSO> levelEffects = building.effects;
        int currentLevel = building.level;
        for(int i = 0; i < levelEffects.Count; i++)
        {
            levelsList[i].transform.GetChild(0).gameObject.SetActive((i + 1 == currentLevel));

            string levelDescription = levelEffects[i].positiveDescription;
            levelDescription = levelDescription.Replace("$V", levelEffects[i].value.ToString());
            TMP_Text[] texts = levelsList[i].GetComponentsInChildren<TMP_Text>();
            texts[texts.Length - 1].text = levelDescription;
        }

        canvas.gameObject.SetActive(true);
        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {
        canvas.gameObject.SetActive(false);
    }
}
