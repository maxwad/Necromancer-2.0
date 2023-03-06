using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public partial class EnemyManager : ISaveable
{
    [HideInInspector] public int id = -1;

    public int Id
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
        }
    }

    public void SetId(int id)
    {
        if(Id == -1) Id = id;
    }

    public int GetId() => Id;

    private void Start()
    {
        Debug.Log(Id);
    }
    public void Save(SaveLoadManager manager)
    {
        List<EnemySD> saveData = new List<EnemySD>();

        foreach(var enemy in enemiesPointsDict)
            saveData.Add(enemy.Key.SaveEnemy());

        EnemySDWrapper test = new EnemySDWrapper();
        test.enemyList = saveData;

        manager.FillSaveData(Id, test);

        Debug.Log(saveData.Count + " enemies saved.");
    }

    public void Load(SaveLoadManager manager, Dictionary<int, object> state)
    {
        if(state.ContainsKey(Id) == false)
        {           
            manager.LoadDataComplete("WARNING: no data about enemies");
            return;
        }

        enemyArragement.SetManager(this);
        EnemySDWrapper saveData = manager.ConvertToRequiredType<EnemySDWrapper>(state[Id]);

        foreach(var enemy in saveData.enemyList)
        {
            EnemyArmyOnTheMap newEnemy = enemyArragement.CreateUsualEnemy(enemy.position.ToVector3(), false);
            newEnemy.LoadEnemy(enemy);
        }

        enemyArragement.ReloadArmies();

        manager.LoadDataComplete("Enemies are loaded (" + saveData.enemyList.Count + ")");
    }
}
