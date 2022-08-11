using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private SpriteRenderer unitSprite;
    private Animator animator;
    private SpriteRenderer countLabel;
    private MeshRenderer unitCountMesh;

    private Vector2 currentDirection;
    private bool currentFacing = false;

    private BattleArmyController armyController;

    private void Start()
    {
        unitSprite    = GetComponent<SpriteRenderer>();
        animator      = GetComponent<Animator>();
        countLabel    = transform.Find("CountBG").GetComponent<SpriteRenderer>();
        unitCountMesh = GetComponentInChildren<MeshRenderer>();

        armyController = GlobalStorage.instance.battlePlayer;

        CheckDirectoin();
        Animamion();
    }

    void Update()
    {
        CheckDirectoin();

        Animamion();
    }

    private void CheckDirectoin()
    {
        currentDirection = armyController.GetArmyDirection();
        currentFacing = armyController.GetArmyFacing();
    }

    private void Animamion()
    {
        unitSprite.flipX = currentFacing;

        int sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
        unitSprite.sortingOrder = sortingOrder;
        countLabel.sortingOrder = sortingOrder;
        unitCountMesh.sortingOrder = sortingOrder;

        bool runningFlag = currentDirection != Vector2.zero ? true : false;
        animator.SetBool(TagManager.A_RUN, runningFlag);
    }
}
