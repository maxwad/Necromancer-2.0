using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendTowerController : MonoBehaviour
{
    private ObjectsPoolManager poolManager;
    [SerializeField] private GameObject[] shootingPoints;
    private GameObject player;
    private GameObject effectsContainer;
    //[SerializeField] private GameObject cannonball;

    [SerializeField] private GameObject startDust;

    public float shootingDelay = 3f;
    private float currentDelay = 0;
    private float coroutineStep = 0.1f;
    private Coroutine coroutine;

    private void Awake()
    {
        effectsContainer = GlobalStorage.instance.effectsContainer;
        poolManager = GlobalStorage.instance.objectsPoolManager;

        coroutineStep = shootingDelay;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(TagManager.T_PLAYER) == true)
        {
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag(TagManager.T_PLAYER) == true)
        {
            player = null;
        }
    }

    private IEnumerator Reloading()
    {
        WaitForSeconds delay = new WaitForSeconds(coroutineStep);

        while(true)
        {
            yield return delay;

            currentDelay += coroutineStep;

            if(currentDelay >= shootingDelay && player != null)
            {
                Shoot();
                currentDelay = 0;
            }
        }
    }

    private void Shoot()
    {
        Vector3 shootingPoint = Vector3.zero;
        float minDistance = 9999;
        for(int i = 0; i < shootingPoints.Length; i++)
        {
            float currentDistance = Vector3.Distance(shootingPoints[i].transform.position, player.transform.position);

            if(Vector3.Distance(shootingPoints[i].transform.position, player.transform.position) < minDistance)
            {
                shootingPoint = shootingPoints[i].transform.position;
                minDistance = currentDistance;
            }
        }

        CreateBullet(shootingPoint);
        CreateEffect(shootingPoint);        
    }

    private void CreateBullet(Vector3 point)
    {
        //GameObject bullet = Instantiate(cannonball);
        GameObject bullet = poolManager.GetObject(NameManager.ObjectPool.Cannonball);
        bullet.transform.position = point;
        bullet.SetActive(true);
        //bullet.transform.SetParent(effectsContainer.transform);
        bullet.GetComponent<CannonballController>().Initialize(player.transform.position);
    }

    private void CreateEffect(Vector3 point)
    {
        GameObject dust = Instantiate(startDust);
        dust.transform.position = point;
        dust.transform.SetParent(effectsContainer.transform);
        PrefabSettings settings = dust.GetComponent<PrefabSettings>();

        if(settings != null) settings.SetSettings(color: Color.white, sortingOrder: 11, sortingLayer: TagManager.T_PLAYER, animationSpeed: 0.05f);
    }
    private void Victory()
    {
        if(coroutine != null) StopCoroutine(coroutine);
    }

    private void OnEnable()
    {
        EventManager.Victory += Victory;

        coroutine = StartCoroutine(Reloading());
    }

    private void OnDisable()
    {
        EventManager.Victory -= Victory;
    }
}
