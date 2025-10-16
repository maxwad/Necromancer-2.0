using System.Collections;
using UnityEngine;

public class FireballWeapon : EnemyWeapon
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected MonoBehaviour fireballPrefab;
    [SerializeField] protected int bulletCount = 3;
    [SerializeField] protected float wideSector = 81;

    private bool isWatching = false;
    private float currentAngle = 0f;

    private GameObject player;
    private Coroutine appearing;

    protected override void SpecialEnableEffect()
    {
        isWatching = true;
        player = hero.gameObject;
        appearing = StartCoroutine(Fade(false));
    }

    private void Update()
    {
        if(isWatching)
        {
            if(player == null)
            {
               return;
            }

            Vector3 targ = player.transform.position;
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

        if(!fadeMode)
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
                currentValue -= step;
                spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentValue);
                yield return new WaitForSeconds(step);
            }

            gameObject.SetActive(false);
        }
    }

    protected override IEnumerator Attack()
    {
        while(currentShoot < countOfShoot)
        {
            yield return new WaitForSeconds(attackDelay - timeOffset);

            Shot();

            currentShoot++;
            currentTime += (attackDelay - timeOffset);
        }

        while(currentTime < attackPeriod)
        {
            yield return new WaitForSeconds(attackPeriod - currentTime);
            currentTime += (attackPeriod - currentTime);
        }

        gameObject.SetActive(false);
    }

    protected override void Shot()
    {
        float startAngle = (currentAngle + wideSector / 2) - 180f;
        float angleStep = wideSector / (bulletCount - 1);

        for(int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + angleStep * i;

            MonoBehaviour currentBullet = objectsPool.GetOrCreateElement(fireballPrefab, this, effectsContainer.transform, false);
            currentBullet.transform.position = transform.position;
            currentBullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            currentBullet.gameObject.SetActive(true); // включается после поворота, иначе угол всегда будет нулевым
        }
    }

    protected override void Stop()
    {
        isWatching = false;
        if(appearing != null)
        {
            StopCoroutine(appearing);
        }

        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        objectsPool.DiscardAll(this);
        gameObject.SetActive(false);
    }

    protected override void SpecialDisableEffect()
    {
        isWatching = false;

        if(appearing != null)
        {
            StopCoroutine(appearing);
        }
    }
}
