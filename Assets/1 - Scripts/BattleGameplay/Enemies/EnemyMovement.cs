using Enums;
using UnityEngine;
using Zenject;

public class EnemyMovement : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rbEnemy;
    private Collider2D collEnemy;
    private SpriteRenderer sprite;
    private BoostManager boostManager;
    private SimpleAnimator animator;

    private float speedBase = 0.1f;
    private float speed;
    private float stuckTime = 0;
    private float maxStuckTime = 1f;
    private bool canICheckStucking = false;
    private Vector3 unStackVector = Vector3.zero;
    private Vector3[] unStackVectors = new Vector3[2];
    private float unstackMultiplier = 7f;

    //for playmode: 10, for editor: 3
    private float acceleration = 10f;

    private bool canIMove = true;
    private float timeAutoEnabling = 1f;

    [Inject]
    public void Construct(BattleArmyController player, BoostManager boostManager)
    {
        this.player = player.gameObject;
        this.boostManager = boostManager;

        collEnemy = GetComponent<Collider2D>();
        rbEnemy = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<SimpleAnimator>();
    }

    private void OnEnable()
    {
        EventManager.SetBattleBoost += UpgradeParameters;

        speed = speedBase + speedBase * boostManager.GetBoost(BoostType.EnemyMovementSpeed);
    }

    private void OnDisable()
    {
        EventManager.SetBattleBoost -= UpgradeParameters;
    }

    private void UpgradeParameters(BoostType boost, float value)
    {
        if(boost == BoostType.EnemyMovementSpeed) speed = speedBase + speedBase * value;
    }

    void Update()
    {
        if(canIMove == true)
            Moving();
        else
            rbEnemy.velocity = Vector2.zero;

        if(canICheckStucking == true) stuckTime += Time.deltaTime;
    }

    private void Moving()
    {
        if(player.transform.position.x - transform.position.x > 0)
            sprite.flipX = true;
        else
            sprite.flipX = false;
        Vector3 movementVector = (player.transform.position - transform.position).normalized + unStackVector;
        rbEnemy.AddForce(movementVector * Time.fixedDeltaTime * acceleration * speed,
        ForceMode2D.Impulse);

        rbEnemy.velocity = Vector3.ClampMagnitude(rbEnemy.velocity, speed);

        if(transform.position.z != 20)
        {
            Debug.Log("2 ENEMY " + gameObject.name + " IS LOST! Z = " + transform.position.z + " local = " + transform.localPosition.z);
            transform.position = new Vector3(transform.position.x, transform.position.y, 20f);
        }
    }

    public void MakeMeFixed(bool mode = false, bool autoEnable = false)
    {
        canIMove = !mode;
        animator.StopAnimation(mode);

        if(autoEnable == true) Invoke("MakeMeUnfixed", timeAutoEnabling);
    }

    //FOR INVOKE
    private void MakeMeUnfixed()
    {
        MakeMeFixed(false);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag(TagManager.T_OBSTACLE) == true)
        {
            canICheckStucking = true;

            if(stuckTime > maxStuckTime)
            {
                unStackVectors[0] = Vector3.Cross(player.transform.position - transform.position, Vector3.one).normalized;
                unStackVectors[1] = -Vector3.Cross(player.transform.position - transform.position, Vector3.up).normalized;

                int index = Random.Range(0, unStackVectors.Length);
                unStackVector = new Vector3(unStackVectors[index].x, unStackVectors[index].y, 0) * unstackMultiplier;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag(TagManager.T_OBSTACLE) == true)
        {
            canICheckStucking = false;
            stuckTime = 0;
            unStackVector = Vector3.zero;
        }
    }

    public void StopMoving(bool mode)
    {
        canIMove = !mode;
    }

    public void BoostSpeed(float boost)
    {
        speedBase += (speedBase * boost);
    }

    public void ResetSpeed()
    {
        speedBase = 1;
    }
}
