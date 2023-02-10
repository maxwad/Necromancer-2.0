using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class ResourceObject : MonoBehaviour
{
    [HideInInspector] public ResourceType resourceType;
    private SpriteRenderer sprite;
    public float quantity;

    private RewardManager rewardManager;
    [HideInInspector] public Reward reward;
    private ResourcesManager resourcesManager;
    private MapBonusManager mapBonusManager;
    private ObjectsPoolManager poolManager;
    private Dictionary<ResourceType, Sprite> resourcesIcons;

    private void OnEnable()
    {
        if(sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
            rewardManager = GlobalStorage.instance.rewardManager;
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
            poolManager = GlobalStorage.instance.objectsPoolManager;
        }

        Birth();
    }

    private IEnumerator Initialize()
    {
        while(GlobalStorage.instance == null || GlobalStorage.instance.isGameLoaded == false)
        {
            yield return null;
        }

        int index = UnityEngine.Random.Range(0, Enum.GetValues(typeof(ResourceType)).Length);
        if(index > 4) index = 0;
        resourceType = (ResourceType)Enum.GetValues(typeof(ResourceType)).GetValue(index);

        sprite.sprite = resourcesIcons[resourceType];

        reward = rewardManager.GetHeapReward(resourceType);
        quantity = reward.resourcesQuantity[0];
    }

    private void Birth()
    {
        StartCoroutine(Initialize());
        StartCoroutine(Blink(true));
    }

    public void Death()
    {
        StartCoroutine(Blink(false));
    }

    private IEnumerator Blink(bool isBorning)
    {
        WaitForSeconds appearDelay = new WaitForSeconds(0.02f);
        float alfaFrom = 0;
        float alfaTo = 1;
        float step = 0.075f;

        if(isBorning == false)
        {
            alfaFrom = 1;
            alfaTo = 0;
            step = -step;

            mapBonusManager.DeleteHeap(this);
        }

        Color currentColor = sprite.color;
        currentColor.a = alfaFrom;
        sprite.color = currentColor;

        bool stop = false;

        while(stop == false)
        {
            alfaFrom += step;
            currentColor.a = alfaFrom;
            sprite.color = currentColor;

            if(step > 0)
            {
                if(alfaFrom >= alfaTo) stop = true;
            }
            else
            {
                if(alfaFrom <= alfaTo) stop = true;
            }

            yield return appearDelay;
        }

        if(isBorning == false) {            
            gameObject.SetActive(false);
        } 
    }

    public void SetMapBonusManager(MapBonusManager mbManager)
    {
        mapBonusManager = mbManager;
    }

    public void GetReward(bool isPlayerPickuping = true)
    {
        if(reward == null)
        {
            return;
        }

        if(isPlayerPickuping == true)
            EventManager.OnResourcePickedUpEvent(reward.resourcesList[0], reward.resourcesQuantity[0]);

        ShowReward(reward, isPlayerPickuping);
        reward = null;
        Death();
    }

    private void ShowReward(Reward reward, bool isPlayerPickuping = true)
    {
        Sprite sprite = resourcesIcons[reward.resourcesList[0]];
        float quantity = reward.resourcesQuantity[0];

        if(isPlayerPickuping == true)
        {
            BonusTipUIManager.ShowVisualEffect(sprite, quantity);
        }
        else
        {
            BonusTipUIManager.ShowVisualEffectForEnemy(sprite, quantity, transform.position);
        }
    }
}
