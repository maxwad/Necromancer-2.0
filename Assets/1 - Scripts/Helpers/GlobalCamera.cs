using UnityEngine;

public class GlobalCamera : MonoBehaviour
{
    private GameObject globalPlayer;

    public float minZOffset = 10;
    public float maxZOffset = 50;
    private float zoom = 0.25f;

    public float moveSpeedMin = 25f;
    public float moveSpeedMax = 300f;

    public float rotationSpeed = 90f;
    private float rotationAngle;

    public float maxPositionX = 800;
    public float maxPositionY = 550;

    private Camera mainCamera;

    private void Awake()
    {
        Camera mainCamera = Camera.main;
    }

    private void Update()
    {
        if(MenuManager.isGamePaused != true && MenuManager.isMiniPause != true)
        {
            float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
            if(zoomDelta != 0f) ChangeZoom(zoomDelta);

            float deltaRotation = Input.GetAxisRaw("Rotation");
            if(deltaRotation != 0f) ChangeRotation(deltaRotation);

            float deltaX = Input.GetAxisRaw("Horizontal");
            float deltaY = Input.GetAxisRaw("Vertical");
            if(deltaX != 0 || deltaY != 0) ChangePosition(deltaX, deltaY);
        }
    }

    private void ChangeRotation(float deltaRotation)
    {
        rotationAngle += deltaRotation * rotationSpeed * Time.deltaTime;

        if(rotationAngle < 0f) rotationAngle += 360f;
        else if(rotationAngle >= 360) rotationAngle -= 360;

        transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
    }

    private void ChangePosition(float deltaX, float deltaY)
    {
        float distance = Mathf.Lerp(moveSpeedMin, moveSpeedMax, zoom) * Time.deltaTime;
        Vector3 direction = transform.localRotation * new Vector3(deltaX, deltaY, 0).normalized;

        Vector3 position = transform.position;
        position += direction * distance;
        transform.position = position;

        transform.position = ClampPosition(position);
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, 0f, maxPositionX);         
        position.y = Mathf.Clamp(position.y, 0f, maxPositionY);
        return position;
    }

    private void ChangeZoom(float zoomDelta)
    {
        zoom = Mathf.Clamp01(zoom - zoomDelta);

        float distance = Mathf.Lerp(minZOffset, maxZOffset, zoom);
        mainCamera.orthographicSize = distance;
    }

    public void SetGlobalCamera(Vector3 startPosition, Vector3 startRotation, float size, GameObject player)
    {
        globalPlayer = player;

        if(mainCamera == null) mainCamera = Camera.main;
        
        if(startPosition != Vector3.zero) transform.position = startPosition;
        if(startRotation != Vector3.zero) transform.eulerAngles = startRotation;
        mainCamera.orthographicSize = size;
    }


}
