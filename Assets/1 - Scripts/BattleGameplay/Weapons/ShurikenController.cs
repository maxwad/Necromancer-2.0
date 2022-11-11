using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 direction;
    public float movementSpeed = 20;
    public float rotationSpeed = 2;

    public float damage = 100;

    private void OnEnable()
    {
        EventManager.EndOfBattle += Stop;

        if(rb == null) rb = GetComponent<Rigidbody2D>();
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
