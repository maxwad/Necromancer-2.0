using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

public class OpeningBuildingWindow : MonoBehaviour
{
    private FortressBuildings allBuildings;

    [SerializeField] private CanvasGroup canvas;

    [SerializeField] private TMP_Text caption;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private List<GameObject> levelsList;

    private GameObject currentBuilding;

    [Inject]
    public void Construct(FortressBuildings allBuildings)
    {
        this.allBuildings = allBuildings;
    }

    public void Open(FBuilding building)
    {
        caption.text = building.buildingName;

        if(building.isSpecialBuilding == true)
        {
            statusText.gameObject.SetActive(false);
            currentBuilding = building.specialFunctional.Init(building);
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
        allBuildings.ShowAllBuildings(true);

        if(currentBuilding != null)
        {
            currentBuilding.SetActive(false);
            currentBuilding = null;
        }
            canvas.gameObject.SetActive(false);
    }
}
