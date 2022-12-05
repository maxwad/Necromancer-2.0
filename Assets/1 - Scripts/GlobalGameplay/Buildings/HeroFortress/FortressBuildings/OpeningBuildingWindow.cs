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
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private List<GameObject> levelsList;

    [SerializeField] private List<GameObject> activeScreens;
    [SerializeField] private Market market;
    [SerializeField] private Military military;


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
            statusText.gameObject.SetActive(false);
            //Enabling active component
            if(building.building == CastleBuildings.Market)
                market.Init();

            if(building.building == CastleBuildings.Barracks
               || building.building == CastleBuildings.TrainingCamp
               || building.building == CastleBuildings.MagicAcademy)
            {
                military.Init(building.building);
            }
        }
        else
        {
            statusText.gameObject.SetActive(true);
            FortressUpgradeSO bonus = allBuildings.GetBuildingBonus(building.building, 1);
            statusText.text = bonus.BuildingDescription;
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
