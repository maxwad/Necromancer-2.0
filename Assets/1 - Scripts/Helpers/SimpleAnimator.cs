using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class SimpleAnimator : MonoBehaviour
{
    public List<Sprite> spriteList;
    public List<Sprite> spriteListAttack;
    private List<Sprite> currentSpriteList = new List<Sprite>();

    public AfterAnimation actionAfterAnimation;
    private SpriteRenderer image;
    private Sprite startImage;
    public float framerate = 0.01f;
    private WaitForSeconds waitTime;
    private Coroutine animating;
    private bool stopAnimation = false;

    private void Awake()
    {
        image = GetComponent<SpriteRenderer>();
        startImage = image.sprite; 
    }

    private void OnEnable()
    {
        currentSpriteList = spriteList;
        if (animating != null) StopCoroutine(animating);

        animating = StartCoroutine(Animate());
    }

    private void OnDisable()
    {
        ResetAnimation();
    }

    private IEnumerator Animate()
    {
        waitTime = new WaitForSeconds(framerate);

        while (true)
        {            
            foreach (Sprite item in currentSpriteList)
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

            if (actionAfterAnimation == AfterAnimation.Destroy)
            {
                DestroyObject();
                break;
            }

            if (actionAfterAnimation == AfterAnimation.SetDisable)
            {
                DisableObject();
                break;
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
    }

    public void ChangeAnimation(Animations animation)
    {
        if(animating != null) StopCoroutine(animating);

        if(animation == Animations.Attack && spriteListAttack.Count != 0) currentSpriteList = spriteListAttack;

        if(animation == Animations.Walk && spriteList.Count != 0) currentSpriteList = spriteList;

        if(gameObject.activeInHierarchy == true) animating = StartCoroutine(Animate());
    }

    public void ChangeAnimation(List<Sprite> newSpriteList)
    {
        if(animating != null) StopCoroutine(animating);

        spriteList = newSpriteList;
        currentSpriteList = newSpriteList;

        if(gameObject.activeInHierarchy == true) animating = StartCoroutine(Animate());

    }

    public void SetSpeed(float newSpeed)
    {
        if(animating != null) StopCoroutine(animating);

        framerate = newSpeed;
        animating = StartCoroutine(Animate());
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }

}
