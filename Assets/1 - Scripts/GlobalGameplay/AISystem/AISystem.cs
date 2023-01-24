using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class AISystem : MonoBehaviour
{
    [SerializeField] private List<Color> castleColors;
    [SerializeField] private int countOfActiveVassals = 3; 

    private Dictionary<EnemyCastle, bool> castles = new Dictionary<EnemyCastle, bool>();
    private List<EnemyCastle> inactiveCastles = new List<EnemyCastle>();
    private int countOfCastles = 0;

    public void RegisterCastle(GameObject castle)
    {
        EnemyCastle newCastle = castle.GetComponent<EnemyCastle>();
        if(newCastle != null)
        {
            castles.Add(newCastle, false);
            inactiveCastles.Add(newCastle);

            Color castleColor = new Color(Random.value, Random.value, Random.value);

            if(castleColors.Count > countOfCastles)
                castleColor = castleColors[countOfCastles];

            newCastle.Init(countOfCastles, castleColor);

            Debug.Log("Init castle #" + countOfCastles);
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

    private void OnEnable()
    {
        EventManager.NewMove += CheckStatus;
    }

    private void OnDisable()
    {
        EventManager.NewMove -= CheckStatus;
    }
}
