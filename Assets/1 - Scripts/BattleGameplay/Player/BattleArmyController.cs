using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class BattleArmyController : MonoBehaviour
{
    private Rigidbody2D rbPlayer;
    private float speed = 0;

    [HideInInspector] public Vector2 currentDirection;
    [HideInInspector] public bool currentFacing = false;
    private bool isMovementInverted = false;

    private void Start()
    {
        rbPlayer = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (MenuManager.isGamePaused == false && MenuManager.isMiniPause == false)
        {
            float horizontalMovement = Input.GetAxisRaw("Horizontal");
            float verticalMovement = Input.GetAxisRaw("Vertical");

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
        GlobalStorage.instance.hero.gameObject.GetComponent<HeroMovement>().Dizziness(mode);
    }

    private void SetStartSpeed(PlayersStats stats, float value)
    {        
        if(stats == PlayersStats.Speed) speed = value;
    }

    private void UpgradeSpeed(PlayersStats stats, float value)
    {
        if(stats == PlayersStats.Speed) speed = value;        
    }

    private void OnEnable()
    {
        EventManager.SetStartPlayerStat += SetStartSpeed;
        EventManager.NewBoostedStat += UpgradeSpeed;
    }

    private void OnDisable()
    {
        EventManager.SetStartPlayerStat -= SetStartSpeed;
        EventManager.NewBoostedStat -= UpgradeSpeed;
    }
}
