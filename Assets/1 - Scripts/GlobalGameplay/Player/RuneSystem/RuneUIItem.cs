using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static NameManager;

public class RuneUIItem : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private RuneSO rune;
    [SerializeField] private Image lockImage;
    [SerializeField] private Image icon;
    [SerializeField] private Image bg;

    [SerializeField] private Color bronze;
    [SerializeField] private Color silver;
    [SerializeField] private Color gold;

    [SerializeField] private InfotipTrigger infotip;
    private RunesWindow runesWindow;
    private Canvas dragdrop;


    public void Init(RuneSO currentRune)
    {        
        if(runesWindow == null)
        {
            infotip = GetComponent<InfotipTrigger>();
            runesWindow = GlobalStorage.instance.playerMilitaryWindow.GetComponentInChildren<RunesWindow>();
            dragdrop = GlobalStorage.instance.playerMilitaryWindow.GetComponentInChildren<Canvas>();
        }

        infotip.SetRune(currentRune);

        rune = currentRune;
        icon.sprite = rune.activeIcon;

        if(rune.level == 1) bg.color = bronze;
        if(rune.level == 2) bg.color = silver;
        if(rune.level == 3) bg.color = gold;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        runesWindow.EnableGrid(false);
        transform.SetParent(dragdrop.transform, false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        runesWindow.FillRunesStorages();
    }
}
