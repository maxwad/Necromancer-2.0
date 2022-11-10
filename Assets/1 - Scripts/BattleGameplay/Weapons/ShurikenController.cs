using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenController : MonoBehaviour
{
    private Rigidbody2D rb;
    Vector3 direction;
    public float movementSpeed = 20;
    public float rotationSpeed = 2;

    void Start()
    {
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
            collision.gameObject.GetComponent<EnemyController>().Kill();
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
