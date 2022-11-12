using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class FireballsController : MonoBehaviour
{
    private bool isWatching = false;
    private SpriteRenderer spriteRenderer;
    private Coroutine appearing;
    private Coroutine attack;
    private ObjectsPoolManager objectsPool;

    private float currentAngle = 0f;
    public int bulletCount = 3;
    public float wideSector = 90;
    public float shotPause = 4.5f;
    public float lifeTime = 12f;

    private float currentTime = 0f;
    public GameObject fireball;

    private void OnEnable()
    {
        EventManager.EndOfBattle += AbortCoroutine;

        if(GlobalStorage.instance.isGlobalMode == false) Init();
    }

    public void Init()
    {
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            objectsPool = GlobalStorage.instance.objectsPoolManager;
        }

        appearing = StartCoroutine(Fade(false));
        isWatching = true;
        currentTime = 0f;
        attack = StartCoroutine(Attack());
    }

    private void Update()
    {
        if(isWatching == true && GlobalStorage.instance.isGlobalMode == false) 
        {
            Vector3 targ = GlobalStorage.instance.hero.transform.position;
            targ.z = 0f;

            Vector3 objectPos = transform.position;
            targ.x = targ.x - objectPos.x;
            targ.y = targ.y - objectPos.y;

            currentAngle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(45, 0, currentAngle));
        }
    }

    private IEnumerator Fade(bool fadeMode)
    {
        float step = 0.01f;
        float startValue = 0f;
        float endValue = 0.7f;

        if(fadeMode == true)
        {
            startValue = 1f;
            endValue = 0f;
        }

        float currentValue = startValue;

        Color currentColor = spriteRenderer.color;

        if(fadeMode == false)
        {
            while(currentValue <= endValue)
            {
                currentValue += step;
                spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentValue);
                yield return new WaitForSeconds(step);
            }
        }
        else
        {
            while(currentValue >= endValue)
            {
                currentValue -=  step;
                spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentValue);
                yield return new WaitForSeconds(step);
            }

            gameObject.SetActive(false);
        }        
    }

    private IEnumerator Attack()
    {
        isWatching = true;

        yield return new WaitForSeconds(shotPause);

        while(currentTime < lifeTime)
        {
            Shot();
            yield return new WaitForSeconds(shotPause);
            currentTime += shotPause;
        }

        isWatching = false;
        appearing = StartCoroutine(Fade(true));
    }

    private void Shot()
    {
        float startAngle = (currentAngle + wideSector / 2) - 90;
        float angleStep = wideSector / (bulletCount - 1);

        for(int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            GameObject bullet = objectsPool.GetObject(ObjectPool.Fireball);
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            bullet.SetActive(true);
        }
    }

    public void AbortCoroutine()
    {
        isWatching = false;
        if(appearing != null) StopCoroutine(appearing);
        if(attack != null) StopCoroutine(attack);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        EventManager.EndOfBattle -= AbortCoroutine;
    }
}
