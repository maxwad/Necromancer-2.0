using UnityEngine;
using Zenject;

public partial class GlobalCamera : MonoBehaviour, IInputableAxies
{
    private InputSystem inputSystem;

    public float minZOffset = 10;
    public float maxZOffset = 50;
    private float zoom = 0.25f;
    private float edgeGap = 10;

    public float moveSpeedMin = 25f;
    public float moveSpeedMax = 300f;

    public float rotationSpeed = 90f;
    private float rotationAngle;

    public float minPositionX = 10;
    public float minPositionY = 5; 
    public float maxPositionX = 970;
    public float maxPositionY = 585;

    private float inputDeltaX;
    private float inputDeltaY;
    private float inputDeltaRatate;
    private float inputDeltaZoom;
    private bool inputMiddleMB = false;
    private Vector3 inputMousePosition;

    private bool isDrag = false;
    private Vector3 origin;
    private Vector3 difference;

    private Camera mainCamera;
    private GameObject globalPlayer;
    private GameObject observeObject;


    [Inject]
    public void Construct(InputSystem inputSystem, GMPlayerMovement globalPlayer)
    {
        this.inputSystem = inputSystem;
        this.globalPlayer = globalPlayer.gameObject;
    }

    private void Awake()
    {
        Camera mainCamera = Camera.main;

        RegisterInputAxies();

        //for testing
        SetObserveObject();
    }

    public void RegisterInputAxies()
    {
        inputSystem.RegisterInputAxies(this);
    }

    public void InputHandling(AxiesData axiesData, MouseData mouseData)
    {
        inputDeltaX      = axiesData.horValue;
        inputDeltaY      = axiesData.vertData;
        inputDeltaRatate = axiesData.rotData;
        inputDeltaZoom   = axiesData.zoomData;

        inputMousePosition = mouseData.position;
        inputMiddleMB = mouseData.mouseBtnMiddle;

        if(MenuManager.instance.IsTherePauseOrMiniPause() == false)
        {
            if(inputDeltaZoom != 0f) 
                ChangeZoom(inputDeltaZoom);

            if(inputDeltaRatate != 0f) 
                ChangeRotation(inputDeltaRatate);

            if(inputDeltaX != 0 || inputDeltaY != 0) 
                ChangePosition(inputDeltaX, inputDeltaY, moveSpeedMax);

            if(inputMiddleMB == true)
            {
                difference = mainCamera.ScreenToWorldPoint(inputMousePosition) - transform.position;

                if(isDrag == false)
                {
                    isDrag = true;
                    origin = mainCamera.ScreenToWorldPoint(inputMousePosition);
                }
            }
            else
            {
                isDrag = false;
            }

            if(isDrag == true) 
                transform.position = ClampPosition(origin - difference);

            //CheckMouseNearEdge();
        }

    }

    //private void LateUpdate()
    //{
    //    if(MenuManager.instance.IsTherePauseOrMiniPause() == false)
    //    {
    //        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
    //        if(zoomDelta != 0f) ChangeZoom(zoomDelta);

    //        float deltaRotation = Input.GetAxisRaw("Rotation");
    //        if(deltaRotation != 0f) ChangeRotation(deltaRotation);

    //        //float deltaX = Input.GetAxisRaw("Horizontal");
    //        //float deltaY = Input.GetAxisRaw("Vertical");
    //        if(inputDeltaX != 0 || inputDeltaY != 0) ChangePosition(inputDeltaX, inputDeltaY, moveSpeedMax);

    //        if(Input.GetMouseButton(2))
    //        {
    //            difference = mainCamera.ScreenToWorldPoint(Input.mousePosition) - mainCamera.transform.position;

    //            if(isDrag == false)
    //            {
    //                isDrag = true;
    //                origin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    //            }
    //        }
    //        else
    //        {
    //            isDrag = false;
    //        }

    //        if(isDrag == true) mainCamera.transform.position = ClampPosition(origin - difference);

    //        //CheckMouseNearEdge();

    //        //if(observeObject != null) Observation();
    //    }
    //}

    private void CheckMouseNearEdge()
    {
        float deltaX = 0;
        float deltaY = 0;

        if(inputMousePosition.x < edgeGap) 
        {
            deltaX = -1f;
            deltaY = 0f;
        }

        if(inputMousePosition.x > Screen.width - edgeGap)
        {
            deltaX = 1f;
            deltaY = 0f;
        }

        if(inputMousePosition.y < edgeGap)
        {
            deltaX = 0f;
            deltaY = -1f;
        }

        if(inputMousePosition.y > Screen.height - edgeGap)
        {
            deltaX = 0f;
            deltaY = 1f;
        }

        ChangePosition(deltaX, deltaY, moveSpeedMax / 2);
    }

    private void ChangeRotation(float deltaRotation)
    {
        rotationAngle += deltaRotation * rotationSpeed * Time.deltaTime;

        if(rotationAngle < 0f) rotationAngle += 360f;
        else if(rotationAngle >= 360) rotationAngle -= 360;

        transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
    }

    private void ChangePosition(float deltaX, float deltaY, float speed)
    {
        float distance = Mathf.Lerp(moveSpeedMin, speed, zoom) * Time.deltaTime;
        Vector3 direction = transform.localRotation * new Vector3(deltaX, deltaY, 0).normalized;

        Vector3 position = transform.position;
        position += direction * distance;
        transform.position = position;

        transform.position = ClampPosition(position);
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, minPositionX, maxPositionX);         
        position.y = Mathf.Clamp(position.y, minPositionY, maxPositionY);
        return position;
    }

    private void ChangeZoom(float zoomDelta)
    {
        zoom = Mathf.Clamp01(zoom - zoomDelta);

        float distance = Mathf.Lerp(minZOffset, maxZOffset, zoom);
        mainCamera.orthographicSize = distance;
    }

    public void SetGlobalCamera(Vector3 startPosition, Vector3 startRotation, float size)
    {
        if(mainCamera == null) 
            mainCamera = Camera.main;
        
        if(startPosition != Vector3.zero) transform.position = startPosition;
        if(startRotation != Vector3.zero) transform.eulerAngles = startRotation;
        mainCamera.orthographicSize = size;
    }

    public void SetObserveObject(GameObject obsObj = null)
    {
        observeObject = (obsObj == null) ? globalPlayer : obsObj;
        transform.position = new Vector3(observeObject.transform.position.x, observeObject.transform.position.y, transform.position.z);
    }
}
