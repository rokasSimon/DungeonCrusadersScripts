using Assets.Scripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Shatter : MonoBehaviour
{
    [Min(0)]
    [Range(1, 5)]
    [SerializeField]
    private Vector2Int sliceCount = new Vector2Int(2, 3);

    [Min(0)]
    [SerializeField]
    private float shatterForceRadius = 5f;

    [Min(0)]
    [SerializeField]
    private float shatterForce = 300f;

    [SerializeField]
    private bool destroyOnShatter;

    [SerializeField]
    private UnityEvent onShatter;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void ShatterGameObject()
    {
        var meshes = SliceMeshes();
        if (meshes.Count == 0)
        {
            return;
        }

        CreateGameObjects(meshes);
        if (destroyOnShatter)
        {
            DestroyCurrentGameObject();
        }

        onShatter.Invoke();
    }

    private List<SlicedMesh> SliceMeshes()
    {
        var randomSliceCount = Random.Range(sliceCount.x, sliceCount.y);
        return gameObject.Slice(randomSliceCount);
    }

    private void CreateGameObjects(IReadOnlyList<SlicedMesh> meshes)
    {
        var currentTransform = gameObject.transform;
        var currentPosition = currentTransform.position;

        for (int i = 0; i < meshes.Count; i++)
        {
            var newGameObject = CreateGameObject(currentTransform, i);
            var slicedMesh = meshes[i];

            AddMeshFilter(newGameObject, slicedMesh.Mesh);
            AddMeshRenderer(newGameObject, slicedMesh.Material);
            AddMeshCollider(newGameObject);

            var newRigidBody = AddRigidbody(newGameObject, meshes.Count);
            AddShatterForce(newRigidBody, currentPosition);
        }
    }

    private GameObject CreateGameObject(Transform currentTransform, int index)
    {
        return new GameObject
        {
            transform =
            {
                localScale = currentTransform.localScale,
                position = currentTransform.position,
                rotation = currentTransform.rotation,
                parent = currentTransform.parent
            },
            name = $"{gameObject.name} ({index})",
        };
    }

    private void AddMeshFilter(GameObject newGameObject, Mesh mesh)
    {
        var newMeshFilter = newGameObject.AddComponent<MeshFilter>();
        newMeshFilter.sharedMesh = mesh;
    }

    private void AddMeshRenderer(GameObject newGameObject, Material material)
    {
        var newMeshRenderer = newGameObject.AddComponent<MeshRenderer>();
        newMeshRenderer.sharedMaterial = material;
    }

    private void AddMeshCollider(GameObject newGameObject)
    {
        var newMeshCollider = newGameObject.AddComponent<MeshCollider>();
        newMeshCollider.convex = true;
    }

    private Rigidbody AddRigidbody(GameObject newGameObject, int meshCount)
    {
        var newRigidBody = newGameObject.AddComponent<Rigidbody>();
        newRigidBody.angularDrag = rigidbody.angularDrag;
        newRigidBody.mass = rigidbody.mass / meshCount;
        newRigidBody.drag = rigidbody.drag;

        return newRigidBody;
    }

    private void AddShatterForce(Rigidbody newRigidBody, Vector3 position)
    {
        newRigidBody.AddExplosionForce(shatterForce, position, shatterForceRadius);
    }

    private void DestroyCurrentGameObject()
    {
        var currentGameObject = gameObject;
        currentGameObject.SetActive(false);
        Destroy(currentGameObject);
    }
}
