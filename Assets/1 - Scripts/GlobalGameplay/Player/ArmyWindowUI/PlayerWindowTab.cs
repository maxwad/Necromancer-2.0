using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Enums;

public class PlayerWindowTab : MonoBehaviour, IPointerClickHandler
{
    private PlayerPersonalWindow playerPersonalWindow;
    [SerializeField] private PlayersWindow playersWindow;
    [SerializeField] private KeyActions keyAction;

    [Inject]
    public void Construct(PlayerPersonalWindow playerPersonalWindow)
    {
        this.playerPersonalWindow = playerPersonalWindow;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        playerPersonalWindow.HandlingTabs(playersWindow, keyAction);
    }    
}
