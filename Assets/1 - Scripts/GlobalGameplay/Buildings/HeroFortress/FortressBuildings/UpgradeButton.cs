using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using static NameManager;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private FBuilding building;
    [SerializeField] private InfotipTrigger costDescription;
    [SerializeField] private GameObject warningBlock;
    [SerializeField] private TMP_Text requiredLevel;

    //public void SetRequiredLevel(int level)
    //{
    //    requiredLevel.text = level.ToString();
    //}

    public void OnPointerClick(PointerEventData eventData)
    {
        building.StartToBuild();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        costDescription.SetCost(building.GetRequirements());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //warningBlock.SetActive(false);
    }

    private void OnDisable()
    {
        //warningBlock.SetActive(false);
    }
}
