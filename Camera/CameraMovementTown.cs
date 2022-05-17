using System;
using UnityEngine;


public class CameraMovementTown : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] private Vector3 offsetPosition = new Vector3(0f, 3f, 8f);
    [SerializeField] private Space offsetPositionSpace = Space.Self;
    [SerializeField] private bool lookAt = true;

    private void Update()
    {
        if (target is null)
        {
            Debug.Log("SET THE TARGET!");
            return;
        }

        UpdatePosition();
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        if (lookAt)
        {
            transform.LookAt(target);
        }
        else
        {
            transform.rotation = target.rotation;
        }
    }

    private void UpdatePosition()
    {
        if (offsetPositionSpace == Space.Self)
        {
            transform.position = target.TransformPoint(offsetPosition);
        }
        else
        {
            transform.position = target.position + offsetPosition;
        }
    }
}