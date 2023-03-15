using UnityEngine;

public class PortalBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;
    private PortalsManager portalsManager;

    [SerializeField] private GameObject portalsContainer;

    public void Build(GlobalMapTileManager manager)
    {
        if(gmManager == null)
        {
            gmManager = manager;
            portalsManager = GlobalStorage.instance.portalsManager;
        }

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
