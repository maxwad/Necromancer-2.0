using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class AISystem : MonoBehaviour
{
    [SerializeField] private List<Color> castleColors;
    [SerializeField] private List<string> castleOwners;
    [SerializeField] private int countOfActiveVassals = 8; 

    private Dictionary<EnemyCastle, bool> castles = new Dictionary<EnemyCastle, bool>();
    private List<GameObject> allCastles = new List<GameObject>();
    private int countOfCastles = 0;

    public void RegisterCastle(GameObject castle)
    {
        EnemyCastle newCastle = castle.GetComponent<EnemyCastle>();
        if(newCastle != null)
        {
            castles.Add(newCastle, false);
            allCastles.Add(castle);

            Color castleColor = Color.black;
            string name = "";

            if(castleColors.Count > countOfCastles)
                castleColor = castleColors[countOfCastles];

            if(castleOwners.Count > countOfCastles)
                name = castleOwners[countOfCastles];

            newCastle.Init(castleColor, name);

            countOfCastles++;
        }
    }

    private void CheckStatus()
    {
        if(castles.Count == 0) return;

        List<EnemyCastle> allCastles = new List<EnemyCastle>();
        int activeVassals = 0;

        foreach(var castle in castles)
        {
            if(castle.Value == false)
                allCastles.Add(castle.Key);
            else
                activeVassals++;
        }

        if(activeVassals < countOfActiveVassals && activeVassals < castles.Count)
        {
            int index = Random.Range(0, allCastles.Count);            
            castles[allCastles[index]] = allCastles[index].Activate();
        }
    }

    public void CrusadeComplete(EnemyCastle castle)
    {
        castles[castle] = false;
    }

    public void CastleIsDestroyed(EnemyCastle castle)
    {
        castles.Remove(castle);

        if(castles.Count == 0)
            Debug.Log("All Vassals are DEAD!");
    }

    public List<GameObject> GetCastles()
    {
        return allCastles;
    }

    private void OnEnable()
    {
        EventManager.NewMove += CheckStatus;
    }

    private void OnDisable()
    {
        EventManager.NewMove -= CheckStatus;
    }
}
