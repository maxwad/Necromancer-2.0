using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BoxObject : MonoBehaviour
{
    private RewardManager rewardManager;
    public Reward reward;
    private MapBoxesManager mbManager;

    private Coroutine deathCoroutine;

    private SpriteRenderer spriteRenderer;
    public Sprite openedSprite;
    private Sprite closedSprite;

    private TooltipTrigger tooltip;

    private void OnEnable()
    {
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            closedSprite = spriteRenderer.sprite;

            tooltip = gameObject.GetComponent<TooltipTrigger>();

            rewardManager = GlobalStorage.instance.rewardManager;
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
            Debug.Log("Reward is wasted!");
            return;
        }

        spriteRenderer.sprite = openedSprite;
        tooltip.SetStatus(true);

        float luck = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Luck);
        float multiplier = 1;
        if(Random.Range(0, 101) <= luck)
        {
            multiplier += GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.DoubleBonusFromBox);
        }

        // Handling exp

        for(int i = 0; i < reward.resourcesList.Count; i++)
        {
            EventManager.OnResourcePickedUpEvent(reward.resourcesList[i], reward.resourcesQuantity[i] * multiplier);
        }

        reward = null;
    }
}
