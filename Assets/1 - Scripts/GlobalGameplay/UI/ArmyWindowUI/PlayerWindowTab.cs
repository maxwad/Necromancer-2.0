using UnityEngine;
using UnityEngine.EventSystems;
using static NameManager;

public class PlayerWindowTab : MonoBehaviour, IPointerClickHandler
{
    private PlayerPersonalWindow playerPersonalWindow;
    [SerializeField] private PlayersWindow playersWindow;
    [SerializeField] private KeyCode key;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(playerPersonalWindow == null) playerPersonalWindow = GlobalStorage.instance.playerMilitaryWindow;

        playerPersonalWindow.HandlingTabs(playersWindow, key);
    }    
}
