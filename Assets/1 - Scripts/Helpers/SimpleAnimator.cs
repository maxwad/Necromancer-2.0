using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SimpleAnimator : MonoBehaviour
{
    public List<Sprite> spriteList;
    public List<Sprite> spriteListAttack;
    private List<Sprite> currentSpriteList = new List<Sprite>();

    public AfterAnimationAction actionAfterAnimation;
    private SpriteRenderer image;
    private Sprite startImage;
    public float framerate = 0.01f;
    private WaitForSeconds waitTime;
    private Coroutine animationCor;
    private bool stopAnimation = false;
    private MonoBehaviour prefabSource;
    private MonoBehaviour prefabInstance;

    private ObjectsPoolManager poolManager;

    [Inject]
    public void Construct(ObjectsPoolManager poolManager)
    {
        this.poolManager = poolManager;
    }

    private void Awake()
    {
        image = GetComponent<SpriteRenderer>();
        startImage = image.sprite;
    }

    private void OnEnable()
    {
        currentSpriteList = spriteList;
        if(animationCor != null)
        {
            StopCoroutine(animationCor);
        }

        animationCor = StartCoroutine(Animate());
    }

    private void OnDisable()
    {
        ResetAnimation();
    }

    private void CallAfterAniation(AfterAnimationAction actionType)
    {
        if(prefabSource != null)
        {
            poolManager.DiscardByInstance(prefabInstance, prefabSource);
        }

        EventManager.OnAnimationFinishedEvent(actionAfterAnimation);
    }


    public void SerPrefabSource(MonoBehaviour prefabSource, MonoBehaviour prefabInstance)
    {
        this.prefabSource = prefabSource;
        this.prefabInstance = prefabInstance;
    }

    private IEnumerator Animate()
    {
        waitTime = new WaitForSeconds(framerate);

        while(true)
        {
            foreach(Sprite item in currentSpriteList)
            {
                if(stopAnimation == false)
                {
                    image.sprite = item;
                    if(gameObject.CompareTag(TagManager.T_ENEMY) == true)
                    {
                        image.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
                    }
                }
                yield return waitTime;
            }

            if(actionAfterAnimation != AfterAnimationAction.Nothing)
            {
                CallAfterAniation(actionAfterAnimation);
            }
        }
    }

    public void StopAnimation(bool mode)
    {
        stopAnimation = mode;
    }

    public void ResetAnimation()
    {
        stopAnimation = false;
        image.sprite = startImage;
        currentSpriteList = spriteList;

        prefabSource = null;
    }

    public void ChangeAnimation(Animations animation)
    {
        if(this.animationCor != null)
        {
            StopCoroutine(this.animationCor);
        }

        if(animation == Animations.Attack && spriteListAttack.Count != 0)
        {
            currentSpriteList = spriteListAttack;
        }

        if(animation == Animations.Walk && spriteList.Count != 0)
        {
            currentSpriteList = spriteList;
        }

        if(gameObject.activeInHierarchy)
        {
            this.animationCor = StartCoroutine(Animate());
        }
    }

    public void ChangeAnimation(List<Sprite> newSpriteList)
    {
        if(animationCor != null)
        {
            StopCoroutine(animationCor);
        }

        spriteList = newSpriteList;
        currentSpriteList = newSpriteList;

        if(gameObject.activeInHierarchy)
        {
            animationCor = StartCoroutine(Animate());
        }
    }

    public void SetSpeed(float newSpeed)
    {
        if(animationCor != null)
        {
            StopCoroutine(animationCor);
        }

        framerate = newSpeed;
        animationCor = StartCoroutine(Animate());
    }
}
