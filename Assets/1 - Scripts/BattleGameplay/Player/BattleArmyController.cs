using UnityEngine;
using static NameManager;
using Zenject;

public class BattleArmyController : MonoBehaviour, IInputableAxies
{
    private InputSystem inputSystem;
    private PlayerStats playerStats;

    private Rigidbody2D rbPlayer;
    private HeroController hero;
    private HeroMovement heroMovement;

    private float speedBase;
    private float speed;
    private float inputDeltaX;
    private float inputDeltaY;

    [HideInInspector] public Vector2 currentDirection;
    [HideInInspector] public bool currentFacing = false;
    private bool isMovementInverted = false;

    [Inject]
    public void Construct(
        InputSystem inputSystem,
        HeroController hero,
        PlayerStats playerStats)
    {
        this.inputSystem = inputSystem;
        this.hero        = hero;
        this.playerStats = playerStats;

        rbPlayer = GetComponent<Rigidbody2D>();
        heroMovement = hero.GetComponent<HeroMovement>();

        RegisterInputAxies();
    }

    public void RegisterInputAxies()
    {
        inputSystem.RegisterInputAxies(this);
    }

    public void InputHandling(AxiesData axiesData, MouseData mouseData)
    {
        inputDeltaX = axiesData.horValue;
        inputDeltaY = axiesData.vertData;

        if(MenuManager.instance.IsTherePauseOrMiniPause() == false)
        {
            currentDirection = (hero.IsHeroDead()) ? Vector2.zero : new Vector2(inputDeltaX, inputDeltaY).normalized;

            CheckDirection();

            Moving(currentDirection);
        }
    }

    //private void Awake()
    //{
    //    rbPlayer = GetComponent<Rigidbody2D>();
    //    heroMovement = hero.GetComponent<HeroMovement>();
    //}

    private void CheckDirection()
    {
        if(currentDirection.x < 0) currentFacing = false;
        if(currentDirection.x > 0) currentFacing = true;
    }

    public Vector2 GetArmyDirection()
    {
        return currentDirection;
    }

    public bool GetArmyFacing()
    {
        return currentFacing;
    }

    private void Moving(Vector2 direction)
    {
        direction = isMovementInverted == true ? -direction : direction;
        rbPlayer.velocity = (Vector3)direction * Time.fixedDeltaTime * speed;
    }

    public void MovementInverting(bool mode)
    {
        isMovementInverted = mode;
        heroMovement.Dizziness(mode);
    }

    private void UpgradeSpeed(BoostType boost, float value)
    {
        if(boost == BoostType.MovementSpeed) speed = speedBase + speedBase * value;
    }

    private void OnEnable()
    {
        //if(playerStats == null) 
        //{ 
        //    playerStats = GlobalStorage.instance.playerStats;
        //}

        speedBase = playerStats.GetCurrentParameter(PlayersStats.Speed);
        speed = speedBase;

        MovementInverting(false);

        EventManager.SetBattleBoost += UpgradeSpeed;
    }

    private void OnDisable()
    {
        EventManager.SetBattleBoost -= UpgradeSpeed;
    }
}
