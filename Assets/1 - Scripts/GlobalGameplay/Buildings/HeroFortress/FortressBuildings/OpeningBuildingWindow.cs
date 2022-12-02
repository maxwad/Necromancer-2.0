using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class OpeningBuildingWindow : MonoBehaviour
{
    private FortressBuildings allBuildings;
    private HeroFortress fortress;

    [SerializeField] private CanvasGroup canvas;

    [SerializeField] private TMP_Text caption;
    [SerializeField] private GameObject status;
    [SerializeField] private List<GameObject> levelsList;

    [SerializeField] private List<GameObject> activeScreens;
    [SerializeField] private Market market;


    public void Open(FBuilding building)
    {
        if(allBuildings == null) 
        {
            allBuildings = GlobalStorage.instance.fortressBuildings;
            fortress = GlobalStorage.instance.heroFortress;
        }

        foreach(var screen in activeScreens)
            screen.SetActive(false);

        caption.text = building.building.ToString();

        if(building.isSpecialBuilding == true)
        {
            status.SetActive(false);
            //Enabling active component
            if(building.building == CastleBuildings.Market)
            {
                //market.gameObject.SetActive(true);
                market.Init();
            }
        }
        else
        {
            status.SetActive(true);
        }

        int currentLevel = building.level;
        for(int i = 0; i < levelsList.Count; i++)
        {
            FortressUpgradeSO bonus = allBuildings.GetBuildingBonus(building.building, i + 1);

            levelsList[i].transform.GetChild(0).gameObject.SetActive((i + 1 == currentLevel));

            string levelDescription = bonus.description;
            levelDescription = levelDescription.Replace("$V", bonus.value.ToString());
            TMP_Text[] texts = levelsList[i].GetComponentsInChildren<TMP_Text>();
            texts[texts.Length - 1].text = levelDescription;
        }

        canvas.gameObject.SetActive(true);
        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {
        //TEMPER
        GlobalStorage.instance.heroFortress.ShowAllBuildings(true);
        canvas.gameObject.SetActive(false);
    }
}
