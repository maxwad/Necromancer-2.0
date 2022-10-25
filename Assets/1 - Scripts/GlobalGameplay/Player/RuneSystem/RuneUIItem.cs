using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static NameManager;

public class RuneUIItem : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [HideInInspector] public RuneSO rune;
    public Image lockImage;
    public Image icon;
    public Image bg;

    [SerializeField] private Color bronze;
    [SerializeField] private Color silver;
    [SerializeField] private Color gold;

    [SerializeField] private InfotipTrigger infotip;
    private CanvasGroup canvasGroup;

    private RunesWindow runesWindow;
    private Canvas dragdrop;
    private Transform currentParent;
    private Transform parentStorage;


    public void Init(RuneSO currentRune)
    {        
        if(runesWindow == null)
        {
            infotip = GetComponent<InfotipTrigger>();
            canvasGroup = GetComponent<CanvasGroup>();
            runesWindow = GlobalStorage.instance.playerMilitaryWindow.GetComponentInChildren<RunesWindow>();
            Canvas[] group = GlobalStorage.instance.playerMilitaryWindow.GetComponentsInChildren<Canvas>();
            dragdrop = group[group.Length - 1];
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
        currentParent = transform.parent;

        RunePlaceItem parentPlace = currentParent.GetComponent<RunePlaceItem>();
        if(parentPlace != null)
        {
            parentPlace.ClearCell();
        }
        else
        {
            parentStorage = currentParent;
        }
        transform.SetParent(dragdrop.transform, false);
        canvasGroup.alpha = 0.8f;
        transform.localScale = new Vector3(1f, 1f, 1f);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(transform.parent != dragdrop.transform)
        {
            if(transform.parent != currentParent)
            {            
                currentParent = transform.parent;
                runesWindow.CutRuneFromList(this);
            }
        }
        else
        {
            transform.SetParent(parentStorage);
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        runesWindow.UpdateWindow();
    }
}
