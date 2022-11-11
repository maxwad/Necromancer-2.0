using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Fireball : MonoBehaviour
{
    private Rigidbody2D rb;
    Vector3 direction;
    public float movementSpeed = 20;
    public float rotationSpeed = 2;
    public float playerDamage = 10f;
    public float enemyDamage = 100f;

    public float damage = 100;
    private bool isEnemyDamage = true;

    public void Init(bool targetMode)
    {
        isEnemyDamage = targetMode;

    }

    private void OnEnable()
    {
        EventManager.EndOfBattle += Stop;

        transform.SetParent(GlobalStorage.instance.effectsContainer.transform);

        rb = GetComponent<Rigidbody2D>();
        direction = rb.transform.up * movementSpeed;
    }

    private void Update()
    {
        rb.velocity = direction;
        rb.transform.Rotate(0, 0, rotationSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(TagManager.T_ENEMY) == true)
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(damage / 2, damage / 2, transform.position);
    }

    private void Stop()
    {
        gameObject.SetActive(false);
    }

    private void OnBecameInvisible()
    {
        Stop();
    }

    private void OnDisable()
    {
        EventManager.EndOfBattle -= Stop;
    }
}
