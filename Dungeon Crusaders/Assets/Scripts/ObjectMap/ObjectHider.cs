using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHider : MonoBehaviour
{
    [SerializeField] new Camera camera;
    [SerializeField] LayerMask objectLayer;

    Vector3 previousCameraPosition;
    HashSet<Vector3> prevHidden;

    Dictionary<Vector3, GameObject> hideableObjects;

    public void Init(List<GameObject> objectsToHide)
    {
        hideableObjects = new Dictionary<Vector3, GameObject>(objectsToHide.Count);

        foreach (var item in objectsToHide)
        {
            var pos = item.transform.position;

            hideableObjects.Add(pos, item);
        }
    }

    private void Start()
    {
        previousCameraPosition = camera.transform.position;
    }

    void FixedUpdate()
    {
        var currentCameraPosition = camera.transform.position;

        if (previousCameraPosition != currentCameraPosition)
        {
            var forward = camera.transform.TransformDirection(Vector3.forward);

            if (Physics.Raycast(currentCameraPosition, forward, out var hit, Mathf.Infinity, objectLayer))
            {
                var pos = hit.transform.position;
                var nearbyPositions = Range(pos, 1);
                nearbyPositions.Add(hit.transform.position);

                var objectLocations = ObjectsToReverse(nearbyPositions);
                ReverseActiveState(objectLocations);

                prevHidden = objectLocations;
            }

            previousCameraPosition = currentCameraPosition;
        }
    }

    // Get the XOR of units to hide now and previously
    HashSet<Vector3> ObjectsToReverse(List<Vector3> positions)
    {
        var objectsToReverse = new HashSet<Vector3>();

        foreach (var np in positions)
        {
            if (hideableObjects.ContainsKey(np))
            {
                var obj = hideableObjects[np];

                objectsToReverse.Add(obj.transform.position);
            }
        }

        if (prevHidden != null)
        {
            objectsToReverse.SymmetricExceptWith(prevHidden);
        }

        return objectsToReverse;
    }

    void ReverseActiveState(IEnumerable<Vector3> positions)
    {
        foreach (var pos in positions)
        {
            if (hideableObjects.TryGetValue(pos, out var obj))
            {
                obj.SetActive(!obj.activeSelf);
            }
        }
    }

    List<Vector3> Range(Vector3 pos, int range)
    {
        var tiles = new List<Vector3>(range * 8);

        float startX = pos.x - range;
        float startZ = pos.z - range;
        float endX = pos.x + range;
        float endZ = pos.z + range;

        for (float i = startX; i <= endX; i++)
        {
            for (float j = startZ; j <= endZ; j++)
            {
                var newPos = new Vector3(i, pos.y, j);
                if (!(newPos != pos))
                {
                    tiles.Add(newPos);
                }
            }
        }

        return tiles;
    }
}
