using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BoxObject : MonoBehaviour
{
    private RewardManager rewardManager;
    public Reward reward;
    private MapBoxesManager mbManager;
    private ObjectsPoolManager poolManager;
    private ResourcesManager resourcesManager;

    private Coroutine deathCoroutine;

    private SpriteRenderer spriteRenderer;
    public Sprite openedSprite;
    private Sprite closedSprite;

    private TooltipTrigger tooltip;
    private string visitedContent = "Looks like someone has been here before.";
    private string defaultContent;

    private Dictionary<ResourceType, Sprite> resourcesIcons;
    [SerializeField] private Sprite luckSprite;
    private float multiplier = 1;


    private void OnEnable()
    {
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            closedSprite = spriteRenderer.sprite;

            tooltip = gameObject.GetComponent<TooltipTrigger>();
            defaultContent = tooltip.content;
            rewardManager = GlobalStorage.instance.rewardManager;

            poolManager = GlobalStorage.instance.objectsPoolManager;

            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
        }        

        Birth();
    }

    private void Initialize()
    {
        reward = rewardManager.GetBoxReward();
    }

    private void Birth()
    {
        Initialize();

        spriteRenderer.sprite = closedSprite;
        tooltip.SetStatus(false);
        tooltip.content = defaultContent;

        StartCoroutine(Blink(true));
    }

    public void Death()
    {
        deathCoroutine = StartCoroutine(Blink(false));
    }

    private IEnumerator Blink(bool isBorning)
    {
        while(deathCoroutine != null)
        {
            yield return null;
        }

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

        Color currentColor = spriteRenderer.color;
        currentColor.a = alfaFrom;
        spriteRenderer.color = currentColor;

        bool stop = false;

        while(stop == false)
        {
            alfaFrom += step;
            currentColor.a = alfaFrom;
            spriteRenderer.color = currentColor;

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

        if(isBorning == false)
        {
            gameObject.SetActive(false);
            deathCoroutine = null;
        }
    }

    public void SetMapBoxManager(MapBoxesManager manager)
    {
        mbManager = manager;
    }

    public void GetReward()
    {
        if(reward == null)
        {
            return;
        }

        spriteRenderer.sprite = openedSprite;
        tooltip.SetStatus(true);
        tooltip.content = visitedContent;

        float luck = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Luck);
        multiplier = 1;
        if(Random.Range(0, 101) <= luck)
        {
            multiplier += GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.DoubleBonusFromBox);
        }

        // Handling exp

        for(int i = 0; i < reward.resourcesList.Count; i++)
        {
            EventManager.OnResourcePickedUpEvent(reward.resourcesList[i], reward.resourcesQuantity[i] * multiplier);
        }

        ShowReward(reward, multiplier > 1);
        reward = null;
    }

    private void ShowReward(Reward reward, bool isDouble)
    {        
        for(int counter = -1; counter < reward.resourcesList.Count; counter++)
        {
            Sprite sprite;
            float quantity;

            if(counter == -1)
            {
                if(isDouble == true)
                {
                    sprite = luckSprite;
                    quantity = 0;
                }
                else
                {
                    continue;
                }
            }
            else
            {
                sprite = resourcesIcons[reward.resourcesList[counter]];
                quantity = reward.resourcesQuantity[counter] * multiplier;
            }
            GameObject rewardText = poolManager.GetObject(ObjectPool.BonusText);

            rewardText.transform.position = transform.position;
            rewardText.SetActive(true);
            rewardText.GetComponent<BonusTip>().Iniatilize(counter, sprite, quantity);
        }
    }
}
