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

    [SerializeField] private CanvasGroup canvas;

    [SerializeField] private TMP_Text caption;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private List<GameObject> levelsList;

    [SerializeField] private List<GameObject> activeScreens;
    [SerializeField] private Market market;
    [SerializeField] private Military military;
    [SerializeField] private UnitCenter unitCenter;


    public void Open(FBuilding building)
    {
        if(allBuildings == null) 
        {
            allBuildings = GlobalStorage.instance.fortressBuildings;
        }

        foreach(var screen in activeScreens)
            screen.SetActive(false);

        caption.text = building.buildingName;

        if(building.isSpecialBuilding == true)
        {
            statusText.gameObject.SetActive(false);
            building.specialFunctional.Init(building.building);
            //Enabling active component
            //if(building.building == CastleBuildings.Market)
            //    market.Init(building.building);

            //if(building.building == CastleBuildings.Barracks
            //   || building.building == CastleBuildings.TrainingCamp
            //   || building.building == CastleBuildings.MagicAcademy)
            //{
            //    military.Init(building.building);
            //}

            //if(building.building == CastleBuildings.RecruitmentCenter)
            //{
            //    unitCenter.Init();
            //}
        }
        else
        {
            statusText.gameObject.SetActive(true);
            statusText.text = building.buildingDescr;
        }

        int currentLevel = allBuildings.GetBuildingsLevel(building.building);
        for(int i = 0; i < levelsList.Count; i++)
        {
            FortressUpgradeSO bonus = allBuildings.GetBuildingBonus(building.building, i);

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
        GlobalStorage.instance.fortressBuildings.ShowAllBuildings(true);

        canvas.gameObject.SetActive(false);
    }
}
