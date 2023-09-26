using UnityEngine;
using Zenject;

public class HeroMovement : MonoBehaviour
{
    private Vector2 currentDirection;
    private bool currentFacing = false;
    [SerializeField] private GameObject dizziness;

    private SpriteRenderer sprite;
    private Animator animator;

    private BattleArmyController armyController;

    [Inject]
    public void Construct(BattleArmyController armyController)
    {
        this.armyController = armyController;

        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }
    void Start()
    {
        CheckDirection();
        Animation();
    }

    void Update()
    {
        CheckDirection();

        Animation();
    }

    private void CheckDirection()
    {
        currentDirection = armyController.GetArmyDirection();
        currentFacing = armyController.GetArmyFacing();
    }

    private void Animation()
    {
        sprite.flipX = currentFacing;
        sprite.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);

        bool runningFlag = currentDirection != Vector2.zero ? true : false;
        animator.SetBool(TagManager.A_RUN, runningFlag);
    }

    public void Dizziness(bool mode)
    {
        dizziness.SetActive(mode);
    }
}
