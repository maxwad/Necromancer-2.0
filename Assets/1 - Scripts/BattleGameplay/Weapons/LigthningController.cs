using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LigthningController : MonoBehaviour
{
    [SerializeField] private GameObject dangerZone;
    [SerializeField] private GameObject ray;
    [SerializeField] private Collider2D dangerCollider;

    [SerializeField] float originalSize = 6;
    private float currentSize = 0;
    private float appearTime = 1.5f;
    private float stepTime = 0.01f;

    private float minPlayerDamage = 10f;
    private float maxPlayerDamage = 15f;

    private Coroutine coroutine;

    private void OnEnable()
    {
        EventManager.EndOfBattle += BackToPool;

        transform.localScale = Vector3.zero;
        currentSize = 0;
        //dangerCollider = GetComponent<Collider2D>();
        dangerCollider.enabled = false;

        ray.SetActive(false);

        coroutine = StartCoroutine(Appearing());
    }

    private IEnumerator Appearing()
    {
        WaitForSeconds stepDelay = new WaitForSeconds(stepTime);

        float sizeStep = originalSize / (appearTime / stepTime);

        while(currentSize < originalSize)
        {
            currentSize += sizeStep;
            transform.localScale = new Vector3(currentSize, currentSize, currentSize);
            yield return stepDelay;
        }

        ray.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        dangerCollider.enabled = true;

        yield return new WaitForSeconds(0.2f);

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(TagManager.T_PLAYER) == true )
        {
            float damage = Random.Range(minPlayerDamage, maxPlayerDamage);
            collision.GetComponent<HeroController>().TakeDamage(damage / 2, damage / 2);
        }

        if(collision.CompareTag(TagManager.T_SQUAD) == true)
        {
            float damage = Random.Range(minPlayerDamage, maxPlayerDamage);
            collision.GetComponent<UnitController>().TakeDamage(damage / 2, damage / 2);
        }
    }

    private void BackToPool()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if(coroutine != null) StopCoroutine(coroutine);
        EventManager.EndOfBattle -= BackToPool;
    }
}
