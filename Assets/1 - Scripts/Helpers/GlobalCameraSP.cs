using System.Collections.Generic;
using UnityEngine;

public partial class GlobalCamera : ISaveable
{
    [SerializeField] private int _id = 101;
    private void Start()
    {
        Debug.Log("Camera " + Id);
    }
    public int Id
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
        }
    }

    public void SetId(int id)
    {
        if(Id >= 100) Id = id;
    }

    public int GetId() => Id;

    public void Save(SaveLoadManager manager)
    {
        CameraSD saveData = new CameraSD();

        saveData.position = transform.position.ToVec3();
        saveData.rotation = transform.eulerAngles.ToVec3();
        saveData.rotationAngle = rotationAngle;
        saveData.zoom = zoom;
        saveData.cameraSize = mainCamera.orthographicSize;

        manager.FillSaveData(Id, saveData);
    }

    public void Load(SaveLoadManager manager, Dictionary<int, object> state)
    {
        if(state.ContainsKey(Id) == false)
        {
            manager.LoadDataComplete("WARNING: no data for Camera");
            return;
        }

        CameraSD saveData = manager.ConvertToRequiredType<CameraSD>(state[Id]);

        rotationAngle = saveData.rotationAngle;
        zoom = saveData.zoom;
        SetGlobalCamera(saveData.position.ToVector3(), saveData.rotation.ToVector3(), saveData.cameraSize);

        manager.LoadDataComplete("Camera is loaded");
    }
}
