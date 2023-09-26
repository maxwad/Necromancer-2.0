using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class RuneUIItem : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [HideInInspector] public RuneSO rune;
    public Image lockImage;
    public Image icon;
    public Image bg;

    [SerializeField] private Color bronze;
    [SerializeField] private Color silver;
    [SerializeField] private Color gold;
    [HideInInspector] public Color originalColor;


    [SerializeField] private InfotipTrigger infotip;
    private CanvasGroup canvasGroup;

    private RunesSystem runesManager;
    private RunesWindow runesWindow;
    private Canvas dragdrop;
    private Transform currentParent;
    private Transform parentStorage;

    [Inject]
    public void Construct(RunesSystem runesManager, PlayerPersonalWindow playerMilitaryWindow)
    {
        this.runesManager = runesManager;
        this.runesWindow = playerMilitaryWindow.GetComponentInChildren<RunesWindow>();

        infotip = GetComponent<InfotipTrigger>();
        canvasGroup = GetComponent<CanvasGroup>();
        Canvas[] group = playerMilitaryWindow.GetComponentsInChildren<Canvas>();
        dragdrop = group[group.Length - 1];
    }

    public void Init(RuneSO currentRune)
    {        
        infotip.SetRune(currentRune);

        rune = currentRune;
        icon.sprite = rune.activeIcon;

        if(rune.level == 1) bg.color = bronze;
        if(rune.level == 2) bg.color = silver;
        if(rune.level == 3) bg.color = gold;

        originalColor = bg.color;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(GlobalStorage.instance.IsGlobalMode() == false)
        {
            InfotipManager.ShowWarning("You cannot make any changes during the battle.");
            return;
        }

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
        if(GlobalStorage.instance.IsGlobalMode() == false)
        {
            InfotipManager.ShowWarning("You cannot make any changes during the battle.");
            return;
        }
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(GlobalStorage.instance.IsGlobalMode() == false)
        {
            InfotipManager.ShowWarning("You cannot make any changes during the battle.");
            return;
        }

        if(transform.parent != dragdrop.transform)
        {
            if(transform.parent != currentParent)
            {            
                currentParent = transform.parent;
            }
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        runesWindow.UpdateWindow();
    }

    public void ResetRune()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        transform.SetParent(parentStorage);
        bg.color = originalColor;

        runesManager.ClearCell(rune);
        runesWindow.PasteRuneToList(this);

        runesWindow.UpdateWindow();
    }
}
