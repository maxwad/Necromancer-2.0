using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class CastleSquadSlot : MonoBehaviour, IPointerClickHandler
{
    private IGarrison garrison;

    [SerializeField] private bool isCastlesSquad = true;
    private bool isActive = false;
    private UnitsTypes unitType;

    [SerializeField] private GameObject squad;
    [SerializeField] private Image squadIcon;
    [SerializeField] private TMP_Text amount;
    [SerializeField] private InfotipTrigger infoTip;

    public void Init(IGarrison gar, Unit unit, int quantity)
    {
        garrison = gar;
        infoTip.SetUnit(unit);
        squad.SetActive(true);
        squadIcon.sprite = unit.unitIcon;
        amount.text = quantity.ToString();

        unitType = unit.unitType;
        isActive = true;
    }

    public void Deactivate()
    {
        infoTip.SetUnit(null);
        squad.SetActive(false);

        isActive = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(isActive == true)
            {
                garrison.StartExchange(isCastlesSquad, unitType);
            }
        }
    }
}
