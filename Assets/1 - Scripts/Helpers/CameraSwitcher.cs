using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    // Start options:
    // x:0, y: 0, z: -10
    // size: 10
    [HideInInspector] public bool isSwitching = false;
    [SerializeField] private GameObject veil;
    [SerializeField] private Image fadeScreen;

    [SerializeField] private GameObject battlePlayer;
    [SerializeField] private GameObject globalPlayer;

    private Vector3 globalCameraPosition = Vector3.zero;
    private Vector3 globalCameraRotation = Vector3.zero;
    private float globalCameraSize = 20f;
    private Vector3 battleCameraPosition = new Vector3(0, 0, 10);

    private GlobalCamera globalCameraMode;
    private BattleCamera battleCameraMode;

    [SerializeField] private GameObject MapsCameraContainer;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        globalCameraMode = GetComponent<GlobalCamera>();
        battleCameraMode = GetComponent<BattleCamera>();

        globalCameraMode.SetGlobalCamera(globalCameraPosition, globalCameraRotation, globalCameraSize, globalPlayer);
    }

    public void FadeIn(bool mode)
    {
        StartCoroutine(StartFadeIn(mode));
    }

    private IEnumerator StartFadeIn(bool mode)
    {
        WaitForSecondsRealtime smallWait = new WaitForSecondsRealtime(0.01f);
        WaitForSecondsRealtime bigWait = new WaitForSecondsRealtime(0.25f);

        veil.SetActive(true);
        isSwitching = true;

        float alfa = 0;
        Color currentColor = fadeScreen.color;

        while (alfa < 1)
        {
            alfa += 0.05f;
            currentColor.a = alfa;
            fadeScreen.color = currentColor;

            yield return smallWait;
        }

        //switch camera behavior mode
        if (mode == true)
        {            
            globalCameraMode.enabled = true;
            battleCameraMode.enabled = false;

            mainCamera.transform.SetParent(MapsCameraContainer.transform);
            globalCameraMode.SetGlobalCamera(globalCameraPosition, globalCameraRotation, globalCameraSize, globalPlayer);
        }
        else
        {
            globalCameraPosition = transform.position;
            globalCameraRotation = transform.eulerAngles;
            globalCameraSize = mainCamera.orthographicSize;

            globalCameraMode.enabled = false;
            battleCameraMode.enabled = true;

            mainCamera.transform.SetParent(null);
            battleCameraMode.SetBattleCamera(battleCameraPosition, battlePlayer);
        }

        yield return bigWait;
        GlobalStorage.instance.SetGlobalMode(mode);
        yield return bigWait;

        while (alfa > 0)
        {
            alfa -= 0.05f;
            currentColor.a = alfa;
            fadeScreen.color = currentColor;

            yield return smallWait;
        }

        veil.SetActive(false);
        isSwitching = false;
    }


    private void OnEnable()
    {
        EventManager.ChangePlayMode += FadeIn;
    }

    private void OnDisable()
    {
        EventManager.ChangePlayMode -= FadeIn;
    }
}
