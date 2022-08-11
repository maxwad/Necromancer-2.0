using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyMovement : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rbEnemy;
    private Collider2D collEnemy;
    private SpriteRenderer sprite;

    private float speed = 1f;
    private float originalSpeed;

    //for playmode: 10, for editor: 3
    private float acceleration = 10f;

    private bool canIMove = true;
    private float timeAutoEnebling = 1f;

    void Start()
    {
        player = GlobalStorage.instance.battlePlayer.gameObject;
        collEnemy = GetComponent<Collider2D>();
        rbEnemy = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        originalSpeed = speed;
    }

    void Update()
    {
        if(canIMove == true)
            Moving();
        else
            rbEnemy.velocity = Vector2.zero;
    }

    private void Moving()
    {
        if (player.transform.position.x - transform.position.x > 0)
            sprite.flipX = true;
        else
            sprite.flipX = false;

        rbEnemy.AddForce((player.transform.position - transform.position).normalized
        * Time.fixedDeltaTime * acceleration * speed,
        ForceMode2D.Impulse);       

        rbEnemy.velocity = Vector3.ClampMagnitude(rbEnemy.velocity, speed);
    }

    public void MakeMeFixed(bool mode = false, bool autoEnable = false)
    {
        canIMove = !mode;
        gameObject.GetComponent<SimpleAnimator>().StopAnimation(mode);

        if(autoEnable == true) Invoke("MakeMeUnfixed", timeAutoEnebling);
    }

    private void MakeMeUnfixed()
    {
        MakeMeFixed(false);
    }

    public void StopMoving(bool mode)
    {
        canIMove = !mode;
    }

    public void BoostSpeed(float boost)
    {
        speed += (speed * boost);
    }

    public void ResetSpeed()
    {
        speed = originalSpeed;
    }

    private void OnBecameVisible()
    {
        collEnemy.enabled = true;
    }

    private void OnBecameInvisible()
    {
        collEnemy.enabled = false;
    }
}
