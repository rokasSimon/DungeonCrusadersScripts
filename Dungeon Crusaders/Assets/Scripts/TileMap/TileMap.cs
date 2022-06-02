using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void BuildMesh(bool[,] levelData, float height)
    {
        //var (xMax, zMax) = levelData.Size;
        var xMax = levelData.GetLength(0);
        var zMax = levelData.GetLength(1);

        int vertexCountX = xMax + 1;
        int vertexCountZ = zMax + 1;
        int vertexCount = vertexCountX * vertexCountZ;
        int triangleCount = xMax * zMax * 2;

        var vertexBuffer = new Vector3[vertexCount];
        var normalBuffer = new Vector3[vertexCount];
        var uvBuffer = new Vector2[vertexCount];
        var indexBuffer = new int[triangleCount * 3];

        int uvMod = vertexCountZ > vertexCountX ? vertexCountZ : vertexCountX;

        for (int z = 0; z < vertexCountZ; z++)
        {
            for (int x = 0; x < vertexCountX; x++)
            {
                int i = z * vertexCountX + x;

                vertexBuffer[i] = new Vector3(x, height, z);
                normalBuffer[i] = Vector3.up;
                uvBuffer[i] = new Vector2(x % uvMod, z % uvMod);
            }
        }

        for (int z = 0; z < zMax; z++)
        {
            for (int x = 0; x < xMax; x++)
            {
                if (levelData[x, z])
                {
                    int i = z * xMax + x;
                    int triangleIndex = i * 6;
                    int offset = z * vertexCountX + x;

                    indexBuffer[triangleIndex] = offset;
                    indexBuffer[triangleIndex + 1] = offset + vertexCountX;
                    indexBuffer[triangleIndex + 2] = offset + vertexCountX + 1;

                    indexBuffer[triangleIndex + 3] = offset;
                    indexBuffer[triangleIndex + 4] = offset + vertexCountX + 1;
                    indexBuffer[triangleIndex + 5] = offset + 1;
                }
            }
        }

        var mesh = new Mesh
        {
            vertices = vertexBuffer,
            normals = normalBuffer,
            uv = uvBuffer,
            triangles = indexBuffer
        };

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}
