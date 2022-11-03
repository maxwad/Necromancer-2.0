using UnityEngine;
using static NameManager;

public class BattleArmyController : MonoBehaviour
{
    private Rigidbody2D rbPlayer;
    private HeroController hero;

    private float speedBase;
    private float speed;

    [HideInInspector] public Vector2 currentDirection;
    [HideInInspector] public bool currentFacing = false;
    private bool isMovementInverted = false;

    PlayerStats playerStats;

    private void Start()
    {
        rbPlayer = GetComponent<Rigidbody2D>();
        hero = GetComponentInChildren<HeroController>();
    }

    private void Update()
    {
        if (MenuManager.instance.IsTherePauseOrMiniPause() == false)
        {
            float horizontalMovement = (hero.IsHeroDead()) ? 0 : Input.GetAxisRaw("Horizontal");
            float verticalMovement = (hero.IsHeroDead()) ? 0 : Input.GetAxisRaw("Vertical");

            currentDirection = new Vector2(horizontalMovement, verticalMovement).normalized;

            CheckDirection();

            Moving(currentDirection);
        }
    }

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
        if(GlobalStorage.instance.hero != null)
            GlobalStorage.instance.hero.gameObject.GetComponent<HeroMovement>().Dizziness(mode);
    }

    private void UpgradeSpeed(BoostType boost, float value)
    {
        if(boost == BoostType.MovementSpeed) speed = speedBase + speedBase * value;
    }

    private void OnEnable()
    {
        if(playerStats == null) 
        { 
            playerStats = GlobalStorage.instance.playerStats;
        }

        speedBase = playerStats.GetCurrentParameter(PlayersStats.Speed);
        speed = speedBase;

        EventManager.SetBattleBoost += UpgradeSpeed;
    }

    private void OnDisable()
    {
        EventManager.SetBattleBoost -= UpgradeSpeed;
    }
}
