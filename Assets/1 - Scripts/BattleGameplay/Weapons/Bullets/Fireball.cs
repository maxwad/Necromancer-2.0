using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float movementSpeed = 5;
    [SerializeField] private float rotationSpeed = 3;
    [SerializeField] private float damage = 15;

    private Vector3 direction;

    private void OnEnable()
    {
        direction = rb.transform.up * movementSpeed;

        EventManager.EndOfBattle += Stop;
    }

    private void Update()
    {
        if(MenuManager.instance.IsTherePauseOrMiniPause() == false)
        {
            rb.velocity = direction;
            rb.transform.Rotate(0, 0, rotationSpeed);
            spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(TagManager.T_PLAYER))
        {
            collision.gameObject.GetComponent<HeroController>().TakeDamage(damage / 2, damage / 2);
        }

        if(collision.gameObject.CompareTag(TagManager.T_SQUAD))
        {
            collision.gameObject.GetComponent<UnitController>().TakeDamage(damage / 2, damage / 2);
        }
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
