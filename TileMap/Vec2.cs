using System.Collections.Generic;
using UnityEngine;

public struct Vec2
{
    public int X { get; set; }
    public int Z { get; set; }

    public Vec2 Inv => new (Z, X);
    public List<Vec2> AdjacentTiles => Range(1);

    public Vec2(int x = 0, int z = 0)
    {
        X = x;
        Z = z;
    }

    public Vec2(float x = 0, float z = 0)
    {
        X = (int)x;
        Z = (int)z;
    }

    public Vec2(Vector2 vector2)
    {
        X = (int)vector2.x;
        Z = (int)vector2.y;
    }

    public Vec2(Vector3 vector3)
    {
        X = (int)vector3.x;
        Z = (int)vector3.z;
    }

    public List<Vec2> Range(int range)
    {
        var tiles = new List<Vec2>(range * 8);

        int startX = X - range;
        int startZ = Z - range;
        int endX = X + range;
        int endZ = Z + range;

        for (int i = startX; i <= endX; i++)
        {
            for (int j = startZ; j <= endZ; j++)
            {
                if (!(i == X && j == Z))
                {
                    tiles.Add(new Vec2(i, j));
                }
            }
        }

        return tiles;
    }

    public List<Vec2> RangeOuter(int range)
    {
        var tiles = new List<Vec2>(range * 8);

        var (topX, topZ) = (X + range, Z + range);
        var (botX, botZ) = (X - range, Z - range);

        for (int x = botX; x < topX; x++)
        {
            tiles.Add(new(x, botZ));
            tiles.Add(new(x, topZ));
        }

        for (int z = botZ + 1; z < topZ; z++)
        {
            tiles.Add(new(botX, z));
            tiles.Add(new(topX, z));
        }

        return tiles;
    }

    public List<Vec2> RangeFull(int range)
    {
        var tiles = new List<Vec2>(range * 8);

        int startX = X - range;
        int startZ = Z - range;
        int endX = X + range;
        int endZ = Z + range;

        for (int z = startZ; z <= endZ; z++)
        {
            for (int x = startX; x <= endX; x++)
            {
                tiles.Add(new Vec2(x, z));
            }
        }

        return tiles;
    }

    public override bool Equals(object obj)
    {
        if (obj is Vec2 v)
        {
            return X == v.X && Z == v.Z;
        }

        return false;
    }

    public override int GetHashCode() => (X, Z).GetHashCode();
    public override string ToString()
    {
        return $"({X}, {Z})";
    }

    public static bool operator ==(Vec2 l, Vec2 r) => l.Equals(r);
    public static bool operator !=(Vec2 l, Vec2 r) => !l.Equals(r);
    public void Deconstruct(out int x, out int z)
    {
        x = X;
        z = Z;
    }

    public static implicit operator Vector2(Vec2 v) => new(v.X, v.Z);
}
