using UnityEngine;
using Zenject;

public class PortalBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;
    private PortalsManager portalsManager;

    [SerializeField] private GameObject portalsContainer;

    [Inject]
    public void Construct(PortalsManager portalsManager, GlobalMapTileManager manager)
    {
        this.portalsManager = portalsManager;
        this.gmManager = manager;
    }

    public void Build()
    {
        foreach(Transform child in portalsContainer.transform)
        {
            if(child.GetComponent<ClickableObject>() != null)
            {
                portalsManager.Register(child.gameObject);
                gmManager.AddBuildingToAllOnTheMap(child.gameObject);
            }
        }
    }

    public PortalsSD GetSaveData() => portalsManager.Save();

    public void Load(PortalsSD saveData) => portalsManager.Load(saveData);
}
