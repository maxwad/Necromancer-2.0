using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class GMInterfaceResources : MonoBehaviour
{
    private ObjectsPoolManager poolManager;
    private ResourcesManager resourcesManager;
    public Dictionary<ResourceType, float> resourcesDict = new Dictionary<ResourceType, float>();

    [Header("Resources")]
    [SerializeField] private GameObject resourcesContainer;
    [SerializeField] private TMP_Text goldCount;
    [SerializeField] private TMP_Text foodCount;
    [SerializeField] private TMP_Text woodCount;
    [SerializeField] private TMP_Text stoneCount;
    [SerializeField] private TMP_Text ironCount;
    [SerializeField] private TMP_Text shardsCount;
    [SerializeField] private TMP_Text unitsCount;
    private Dictionary<ResourceType, TMP_Text> resourceCounters = new Dictionary<ResourceType, TMP_Text>();

    [Header("Deltas")]
    [SerializeField] private GameObject goldContainer;
    [SerializeField] private GameObject foodContainer;
    [SerializeField] private GameObject woodContainer;
    [SerializeField] private GameObject stoneContainer;
    [SerializeField] private GameObject ironContainer;
    [SerializeField] private GameObject shardsContainer;
    [SerializeField] private GameObject unitsContainer;
    private Dictionary<ResourceType, GameObject> deltaContainers = new Dictionary<ResourceType, GameObject>();

    private void Awake()
    {
        resourceCounters = new Dictionary<ResourceType, TMP_Text>()
        {
            [ResourceType.Gold]   = goldCount,
            [ResourceType.Food]   = foodCount,
            [ResourceType.Stone]  = stoneCount,
            [ResourceType.Wood]   = woodCount,
            [ResourceType.Iron]   = ironCount,
            [ResourceType.Shards] = shardsCount,
            [ResourceType.Units]  = unitsCount,
        };

        deltaContainers = new Dictionary<ResourceType, GameObject>()
        {
            [ResourceType.Gold]   = goldContainer,
            [ResourceType.Food]   = foodContainer,
            [ResourceType.Stone]  = stoneContainer,
            [ResourceType.Wood]   = woodContainer,
            [ResourceType.Iron]   = ironContainer,
            [ResourceType.Shards] = shardsContainer,
            [ResourceType.Units]  = unitsContainer
        };
    }

    private void Start()
    {
        poolManager = GlobalStorage.instance.objectsPoolManager;
        resourcesManager = GlobalStorage.instance.resourcesManager;

        FillStartResources();
    }


    private void FillStartResources()
    {
        resourcesDict = resourcesManager.GetAllResources();

        foreach(var resource in resourceCounters)
        {
            if(resource.Key != ResourceType.Mana && resource.Key != ResourceType.Health)
            {
                resource.Value.text = Mathf.Round(resourcesDict[resource.Key]).ToString();
            }
        }
    }

    public void ShowDelta(ResourceType resType, float value)
    {
        if(GlobalStorage.instance.isGlobalMode == false) return;

        GameObject delta = poolManager.GetObject(ObjectPool.DeltaCost);
        delta.transform.SetParent(deltaContainers[resType].transform, false);
        delta.GetComponent<DeltaCost>().ShowDelta(value);
    }

    private void FillResource(ResourceType type, float value)
    {
        if(type == ResourceType.Exp || type == ResourceType.Health || type == ResourceType.Mana) return;

        resourceCounters[type].text = Mathf.Round(value).ToString();
    }

    private void OnEnable()
    {
        EventManager.UpgradeResource += FillResource;
    }

    private void OnDisable()
    {
        EventManager.UpgradeResource -= FillResource;
    }
}
