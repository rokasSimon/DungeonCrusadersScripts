using System;
using System.Collections.Generic;

public class UnitLocationController
{
    LevelTileData _tileData;
    Dictionary<Vec2, Unit> _unitLocations;

    public IEnumerable<Unit> Units => _unitLocations.Values;
    public Vec2 LevelSize => _tileData.Size;

    public UnitLocationController(LevelTileData tileData)
    {
        _tileData = tileData;
        _unitLocations = new Dictionary<Vec2, Unit>();
    }

    public bool IsPathable(Vec2 pos)
    {
        // Bounds check
        if (pos.X >= _tileData.Size.X || pos.Z >= _tileData.Size.Z || pos.X < 0 || pos.Z < 0)
        {
            return false;
        }

        if ((_tileData[pos] == TileTypes.Empty || _tileData[pos] == TileTypes.ReservedTile)
             && !_unitLocations.ContainsKey(pos))
        {
            return true;
        }

        return false;
    }

    public bool IsPlayArea(Vec2 pos)
    {
        if (pos.X >= _tileData.Size.X || pos.Z >= _tileData.Size.Z || pos.X < 0 || pos.Z < 0)
        {
            return false;
        }

        if (_tileData[pos] == TileTypes.Empty || _tileData[pos] == TileTypes.ReservedTile)
        {
            return true;
        }

        return false;
    }

    public bool IsInBounds(Vec2 pos)
    {
        return !(pos.X >= _tileData.Size.X || pos.Z >= _tileData.Size.Z || pos.X < 0 || pos.Z < 0);
    }

    public void AddUnit(Unit unit, Vec2 pos)
    {
        if (_unitLocations.ContainsKey(pos))
        {
            throw new ArgumentException($"Unit already present at {pos}");
        }

        _unitLocations.Add(pos, unit);
    }

    public Unit RemoveUnit(Vec2 pos)
    {
        var unit = _unitLocations[pos];

        _unitLocations.Remove(pos);

        return unit;
    }

    public void MoveUnit(Vec2 oldPos, Vec2 newPos)
    {
        var unit = _unitLocations[oldPos];

        _unitLocations.Remove(oldPos);
        _unitLocations.Add(newPos, unit);
    }

    public Unit UnitAt(Vec2 pos)
    {
        if (_unitLocations.ContainsKey(pos))
        {
            return _unitLocations[pos];
        }

        return null;
    }

    public Vec2? FirstOrDefault(TileTypes type)
    {
        var (i, j) = _tileData.Size;

        for (int x = 0; x < i; x++)
        {
            for (int z = 0; z < j; z++)
            {
                if (_tileData.Tiles[x, z] == type)
                {
                    return new Vec2(x, z);
                }
            }
        }

        return null;
    }
}