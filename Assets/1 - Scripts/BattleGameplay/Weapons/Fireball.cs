using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Fireball : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    Vector3 direction;
    private float movementSpeed = 5;

    private float damage = 15;

    private void OnEnable()
    {
        EventManager.EndOfBattle += Stop;

        //transform.SetParent(GlobalStorage.instance.effectsContainer.transform);
        if(rb == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        rb.velocity = transform.right * movementSpeed;
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        //Debug.Log("touch");
        if(collision.gameObject.CompareTag(TagManager.T_PLAYER) == true)
            collision.gameObject.GetComponent<HeroController>().TakeDamage(damage / 2, damage / 2);

        if(collision.gameObject.CompareTag(TagManager.T_SQUAD) == true)
            collision.gameObject.GetComponent<UnitController>().TakeDamage(damage / 2, damage / 2);
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
