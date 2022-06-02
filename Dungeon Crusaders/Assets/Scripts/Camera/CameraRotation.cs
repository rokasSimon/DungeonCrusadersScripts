using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public bool mouseCameraRotationEnabled = true;
    public string cameraRotationButton = "Mouse2";

    private Camera _camera;
    private float x;
    private float y;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (CameraRotationButtonDown())
        {
            Cursor.lockState = CursorLockMode.Locked;
            y = Input.GetAxis("Mouse X");
            _camera.transform.Rotate(Vector3.up * y, Space.World);
        }

        Cursor.lockState = CursorLockMode.None;
    }

    private bool CameraRotationButtonDown()
    {
        if (mouseCameraRotationEnabled)
        {
            int.TryParse(cameraRotationButton[^1..], out var mButton);
            if (Input.GetMouseButton(mButton))
                return true;
        }
        else
        {
            if (Input.GetKeyDown(cameraRotationButton)) return true;
        }

        return false;
    }
}
