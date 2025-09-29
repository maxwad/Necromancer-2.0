using UnityEngine;
using UnityEngine.EventSystems;
using Enums;
using Zenject;

public class ClickableObject : MonoBehaviour
{
    private CursorManager cursorManager;
    private BonusOnTheMapUI bonusOnTheMapUI;
    private EnemyArmyUI enemyArmyUI;
    private PlayerPersonalWindow playerMilitaryWindow;
    private HeroFortress heroFortress;
    private PortalsWindowUI portalDoor;
    private AltarUI altarDoor;
    private ResourceBuildingUI resourceBuildingDoor;
    private TombUI tombDoor;
    private CampUI campDoor;

    public TypeOfObjectOnTheMap objectType;
    public bool isItDoor = false;

    public bool isNormalUIWindow = true;
    public bool canBeOpenedByClick = true;
    public bool canBeOpenedByMove = true;

    public CursorView cursor = CursorView.Default;

    private Vector3 enterPoint;

    [Inject]
    public void Construct(        
        CursorManager cursorManager,
        BonusOnTheMapUI bonusOnTheMapUI,
        EnemyArmyUI enemyArmyUI,
        PlayerPersonalWindow playerMilitaryWindow,
        PortalsWindowUI portalDoor,
        HeroFortress heroFortress,
        AltarUI altarDoor,
        ResourceBuildingUI resourceBuildingDoor,
        TombUI tombDoor,
        CampUI campDoor
        )
    {
        this.cursorManager = cursorManager;
        this.bonusOnTheMapUI = bonusOnTheMapUI;
        this.enemyArmyUI = enemyArmyUI;
        this.playerMilitaryWindow = playerMilitaryWindow;
        this.portalDoor = portalDoor;
        this.heroFortress = heroFortress;
        this.altarDoor = altarDoor;
        this.resourceBuildingDoor = resourceBuildingDoor;
        this.tombDoor = tombDoor;
        this.campDoor = campDoor;
    }

    public void ActivateUIWindow(bool modeClick)
    {
        //mode = true - by rigth click; mode = false - by movement
        if(EventSystem.current.IsPointerOverGameObject()) return;

        if(GlobalStorage.instance.isModalWindowOpen == false)
        {
            if(canBeOpenedByClick == false && modeClick == true) return;
            if(canBeOpenedByMove == false && modeClick == false) return;

            if(isItDoor == false)
            {
                OpenObjectWindow(modeClick, isNormalUIWindow);
            }
            else
            {
                OpenDoorTo(modeClick);
            }                      
        }
    }

    private void OpenObjectWindow(bool modeClick, bool modeUISize)
    {
        bool isThereManager = false;

        switch(objectType)
        {
            case TypeOfObjectOnTheMap.RoadPointer:
                break;

            case TypeOfObjectOnTheMap.Resource:
                bonusOnTheMapUI.OpenWindow(this);
                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.BoxBonus:
                bonusOnTheMapUI.OpenWindow(this);
                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.Enemy:
                enemyArmyUI.OpenWindow(modeClick, gameObject.GetComponent<EnemyArmyOnTheMap>());
                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.Player:
                playerMilitaryWindow.OpenWindow(PlayersWindow.PlayersArmy);
                isThereManager = true;
                break;


            default:
                break;
        }

        if(isThereManager == false) 
        {
            Debug.Log("There was a common manager, but sorry");
        }
    }

    private void OpenDoorTo(bool modeClick)
    {
        bool isThereManager = false;
        EnemyArmyOnTheMap enemy;

        switch(objectType)
        {
            case TypeOfObjectOnTheMap.PlayersCastle:
                heroFortress.Open(modeClick);
                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.NecromancerCastle:
                break;

            case TypeOfObjectOnTheMap.Castle:
                enemy = gameObject.GetComponent<EnemyArmyOnTheMap>();

                if(enemy != null && modeClick == false)
                {
                    enemy.PrepairToTheBattle();
                    isThereManager = true;
                }
                break;

            case TypeOfObjectOnTheMap.Portal:
                portalDoor.Open(modeClick, this.gameObject);
                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.ResourceBuilding:
                ResourceBuilding rBuilding = GetComponent<ResourceBuilding>();
                resourceBuildingDoor.Open(modeClick, rBuilding);
                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.Altar:
                Altar altar = GetComponent<Altar>();
                altarDoor.Open(modeClick, altar);
                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.Tomb:
                enemy = gameObject.GetComponent<EnemyArmyOnTheMap>();

                if(enemy != null && modeClick == false)
                    enemy.PrepairToTheBattle();
                else
                    tombDoor.Open(modeClick, gameObject);               

                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.Camp:

                campDoor.Open(modeClick, gameObject);
                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.Arena:
                break;


            default:
                break;
        }

        if(isThereManager == false)
        {
            Debug.Log("There was a common manager, but sorry 2");
        }
    }

    public Vector3 GetEnterPoint()
    {
        return enterPoint;
    }

    public void SetEnterPoint(Vector3 point)
    {
        enterPoint = point;
    }

    private void OnMouseEnter()
    {
        cursorManager.SetCurrentObjectUnderMouse(this);
    }

    private void OnMouseExit()
    {
        cursorManager.SetCurrentObjectUnderMouse(null);
    }
}
