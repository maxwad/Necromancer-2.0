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
    private Reward reward;
    private ResourcesManager resourcesManager;
    private MapBonusManager mapBonusManager;


    private void OnEnable()
    {
        if(sprite == null) sprite = GetComponent<SpriteRenderer>();

        if(rewardManager == null) rewardManager = GlobalStorage.instance.rewardManager;
        if(resourcesManager == null) resourcesManager = GlobalStorage.instance.resourcesManager;

        Birth();
    }

    private IEnumerator Initialize()
    {
        while(GlobalStorage.instance == null || GlobalStorage.instance.isGameLoaded == false)
        {
            yield return null;
        }

        int index = UnityEngine.Random.Range(0, Enum.GetValues(typeof(ResourceType)).Length);
        if(index == (int)ResourceType.Magic) index = 0;
        resourceType = (ResourceType)Enum.GetValues(typeof(ResourceType)).GetValue(index);

        Sprite icon = resourcesManager.GetAllResourcesIcons()[resourceType];
        sprite.sprite = icon;

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
        WaitForSeconds appearDelay = new WaitForSeconds(0.05f);
        float alfaFrom = 0;
        float alfaTo = 1;
        float step = 0.05f;

        if(isBorning == false)
        {
            alfaFrom = 1;
            alfaTo = 0;
            step = -step;
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
            mapBonusManager.DeleteHeap(this);
            gameObject.SetActive(false);
        } 
    }

    public void SetMapBonusManager(MapBonusManager mbManager)
    {
        mapBonusManager = mbManager;
    }
}
