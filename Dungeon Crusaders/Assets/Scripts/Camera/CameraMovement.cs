using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera lock toggle key")]
    public string cameraLockKey = "c";

    [Header("Camera scroll speed")]
    public float scrollSpeed = 5f;

    [Header("Top trigger barrier (max 1)")]
    public float topLimit = 0.97f;
    [Header("Bottom trigger barrier (max 1)")]
    public float bottomLimit = 0.03f;
    [Header("Left trigger barrier (max 1)")]
    public float leftLimit = 0.97f;
    [Header("Right trigger barrier (max 1)")]
    public float rightLimit = 0.03f;

    private bool _cameraLock;

    // Start is called before the first frame update
    void Start()
    {
        _cameraLock = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(cameraLockKey))
        {
            _cameraLock = _cameraLock ? false : true;
        }

        Vector3 up = new(0f, 1f, 1f);
        Vector3 down = new(0f, -1f, -1f);

        if (!_cameraLock)
        {
            if (Input.mousePosition.y >= Screen.height * topLimit)
                transform.Translate(scrollSpeed * Time.deltaTime * up, Space.Self);
            if (Input.mousePosition.y <= Screen.height * bottomLimit)
                transform.Translate(scrollSpeed * Time.deltaTime * down, Space.Self);
            if (Input.mousePosition.x >= Screen.width * rightLimit)
                transform.Translate(scrollSpeed * Time.deltaTime * Vector3.right, Space.Self);
            if (Input.mousePosition.x <= Screen.width * leftLimit)
                transform.Translate(scrollSpeed * Time.deltaTime * Vector3.left, Space.Self);
        }
    }
}
