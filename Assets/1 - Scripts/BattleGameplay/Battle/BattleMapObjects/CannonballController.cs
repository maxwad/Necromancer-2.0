using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonballController : MonoBehaviour
{
    private Vector3 destination;
    private float speed = 4f;
    [SerializeField] private float minPlayerDamage = 2f;
    [SerializeField] private float maxPlayerDamage = 5f;
    [SerializeField] private float minEnemyDamage = 1f;
    [SerializeField] private float maxEnemyDamage = 2f;
    [SerializeField] private float radiusExplosion = 1f;

    private SpriteRenderer sprite;
    
    private bool canIMove = false;

    [SerializeField] private GameObject death;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(canIMove == true) Movement();
    }

    public void Initialize(Vector3 point)
    {
        destination = point;
        canIMove = true;
    }

    private void Movement()
    {        
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);
        sprite.sortingOrder = -(int)transform.position.y * 100;
        if(destination == transform.position)
        {
            Explosion();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(TagManager.T_PLAYER) == true)
        {
            Explosion();
        }
    }

    private void Explosion()
    {
        CreateEffect();

        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, radiusExplosion);
        foreach(Collider2D obj in objects)
        {
            if(obj.CompareTag(TagManager.T_PLAYER) == true )
            {
                float damage = Random.Range(minPlayerDamage, maxPlayerDamage);
                obj.GetComponent<HeroController>().TakeDamage(damage / 2, damage / 2);
            }

            if(obj.CompareTag(TagManager.T_SQUAD) == true)
            {
                float damage = Random.Range(minPlayerDamage, maxPlayerDamage);
                obj.GetComponent<UnitController>().TakeDamage(damage / 2, damage / 2);
            }

            if(obj.CompareTag(TagManager.T_ENEMY) == true)
            {
                float damage = Random.Range(minEnemyDamage, maxEnemyDamage);
                obj.GetComponent<EnemyController>().TakeDamage(damage / 2, damage / 2, transform.position);
            }
        }

        Destroy(gameObject);
    }

    private void CreateEffect()
    {
        GameObject dust = Instantiate(death);
        dust.transform.position = transform.position;
        dust.transform.SetParent(GlobalStorage.instance.effectsContainer.transform);
        PrefabSettings settings = dust.GetComponent<PrefabSettings>();

        if(settings != null) settings.SetSettings(sortingLayer: TagManager.T_PLAYER);
    }
}
