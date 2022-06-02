using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentMap : MonoBehaviour
{
    [Header("Ground Prefabs")]
    [SerializeField] private GameObject Entrance;
    [SerializeField] private GameObject Exit;
    [SerializeField] private GameObject Pit;
    [SerializeField] private GameObject Ground;

    [Header("Columns")]
    [SerializeField] private GameObject Column;
    [SerializeField] private GameObject TorchColumn;

    [Header("Walls")]
    [SerializeField] private float DefaultWallRotation;
    [SerializeField] private GameObject Wall;
    [SerializeField] private float DefaultCornerRotation;
    [SerializeField] private GameObject WallCorner;

    List<GameObject> _placedObjects;

    public void PlaceObjects(LevelTileData levelData)
    {
        if (_placedObjects != null)
        {
            foreach (GameObject obj in _placedObjects)
            {
                Destroy(obj);
            }
        }

        _placedObjects = new List<GameObject>(/*levelData.Size.X * levelData.Size.Z + levelData.Size.Z * 2 + levelData.Size.X * 2 - 4*/);

        PlaceTileObjects(levelData);
        var corners = PlaceWallCorners(levelData);
        var walls = PlaceWalls(levelData);
        walls.AddRange(corners);

        var objectHider = FindObjectOfType<ObjectHider>();
        objectHider.Init(walls);
    }

    void PlaceTileObjects(LevelTileData levelData)
    {
        for (int x = 0; x < levelData.Size.X; x++)
        {
            for (int z = 0; z < levelData.Size.Z; z++)
            {
                //if (levelData.Tiles[x, z] == TileTypes.Empty || levelData.Tiles[x, z] == TileTypes.ReservedTile) continue;
                if (levelData.Match(x, z, TileTypes.Empty, TileTypes.ReservedTile)) continue;

                var position = new Vector3(x + LevelManager.CenterOffset, LevelManager.LevelHeight, z + LevelManager.CenterOffset);
                var objectToPlace = GetPrefabForTileType(levelData.Tiles[x, z]);

                var obj = Instantiate(objectToPlace, position, RandomRotation.Quaternion, transform);

                if (levelData.Match(x, z, TileTypes.Column))
                {
                    while (IsFacingWall(obj, levelData))
                    {
                        obj.transform.Rotate(new Vector3(0f, 90f, 0f));
                    }
                }

                _placedObjects.Add(obj);
            }
        }
    }

    List<GameObject> PlaceWallCorners(LevelTileData levelData)
    {
        var (x, z) = levelData.Size;
        float offset = LevelManager.CenterOffset;

        var wallCorners = new List<GameObject>();

        _placedObjects.Add(Instantiate(WallCorner,
            new Vector3(0 - offset, LevelManager.LevelHeight, 0 - offset),
            Quaternion.Euler(0, DefaultCornerRotation, 0), transform));
        _placedObjects.Add(Instantiate(WallCorner,
            new Vector3(x + offset, LevelManager.LevelHeight, z + offset),
            Quaternion.Euler(0, DefaultCornerRotation + 180, 0), transform));
        _placedObjects.Add(Instantiate(WallCorner,
            new Vector3(x + offset, LevelManager.LevelHeight, 0 - offset),
            Quaternion.Euler(0, DefaultCornerRotation + 270, 0), transform));
        _placedObjects.Add(Instantiate(WallCorner,
            new Vector3(0 - offset, LevelManager.LevelHeight, z + offset),
            Quaternion.Euler(0, DefaultCornerRotation + 90, 0), transform));

        wallCorners.AddRange(_placedObjects.GetRange(_placedObjects.Count - 4, 4));

        return wallCorners;
    }

    List<GameObject> PlaceWalls(LevelTileData levelData)
    {
        float length = levelData.Size.Z;
        float width = levelData.Size.X;
        float height = LevelManager.LevelHeight;
        float offset = LevelManager.CenterOffset;

        var walls = new List<GameObject>();

        var ranges = new[]
        {
            (1f - offset, 0f - offset, width, Quaternion.Euler(0, DefaultWallRotation + 180f, 0)),
            (0f - offset, 1f - offset, length, Quaternion.Euler(0, DefaultWallRotation + 270f, 0)),
            (0f + offset, length + offset, width, Quaternion.Euler(0, DefaultWallRotation + 360f, 0)),
            (width + offset, 0f + offset, length, Quaternion.Euler(0, DefaultWallRotation + 90f, 0))
        };

        for (int i = 0; i < ranges.Length; i++)
        {
            var (startX, startZ, until, rotation) = ranges[i];

            if (i % 2 == 0)
            {
                for (float x = startX; x < until; x++)
                {
                    var position = new Vector3(x, height, startZ);
                    var obj = Instantiate(Wall, position, rotation, transform);

                    _placedObjects.Add(obj);
                    walls.Add(obj);
                }
            }
            else
            {
                for (float z = startZ; z < until; z++)
                {
                    var position = new Vector3(startX, height, z);
                    var obj = Instantiate(Wall, position, rotation, transform);

                    _placedObjects.Add(obj);
                    walls.Add(obj);
                }
            }
        }

        return walls;
    }

    bool IsFacingWall(GameObject obj, LevelTileData levelData)
    {
        var position = obj.transform.position;
        var forward = obj.transform.forward;
        var forwardOne = position + forward;

        var (maxX, maxZ) = levelData.Size;
        const int minX = 0;
        const int minZ = 0;

        return forwardOne.x > maxX || forwardOne.z > maxZ ||
               forwardOne.x < minX || forwardOne.z < minZ;
    }

    GameObject GetPrefabForTileType(TileTypes type) => type switch
    {
        TileTypes.Exit => Exit,
        TileTypes.Entrance => Entrance,
        TileTypes.Pit => Pit,
        TileTypes.Column => TorchColumn,
        TileTypes.ReservedTile or TileTypes.Empty => Ground,
        _ => throw new System.NotImplementedException()
    };
}
