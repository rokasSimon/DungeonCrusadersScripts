using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class AStar
{
    public static Vec2? AStarSearch(UnitManager context, int range, Vec2 unitPosition, Vec2 enemyPosition)
    {
        var (positions, blocked) = context.GetRawPathing(unitPosition, range);

        int row = blocked.GetLength(0);
        int col = blocked.GetLength(1);

        var destIdx = GetDestinationIndices(positions, enemyPosition, col);

        if (destIdx == null) return null;

        var closedList = new bool[row, col];
        var details = new Cell[row, col];

        for (int z = 0; z < row; z++)
        {
            for (int x = 0; x < col; x++)
            {
                details[x, z] = new Cell();

                details[x, z].Original = positions[z * row + x];
            }
        }

        var (i, j) = (Mathf.FloorToInt(col / 2), Mathf.FloorToInt(col / 2));

        var dest = new Vec2(destIdx.Value.Item2, destIdx.Value.Item1);

        details[i, j] = new Cell { F = 0, G = 0, H = 0, Parent = new(i, j), Original = details[i, j].Original };

        var openList = new SortedSet<Tuple<float, int, int>>
        {
            new Tuple<float, int, int>(0.0f, i, j)
        };

        while (openList.Count != 0)
        {
            var p = openList.Min;

            i = p.Item3;
            j = p.Item2;

            openList.Remove(p);

            closedList[i, j] = true;

            for (int addX = -1; addX <= 1; addX++)
            {
                for (int addY = -1; addY <= 1; addY++)
                {
                    var neighbour = new Vec2(i + addX, j + addY);

                    if (IsValid(blocked, neighbour))
                    {
                        if (neighbour == dest)
                        {
                            details[neighbour.X, neighbour.Z].Parent = new(i, j);

                            var ret = TracePath(details, dest);

                            return ret;
                        }
                        else if (!closedList[neighbour.X, neighbour.Z] && IsNotBlocked(blocked, neighbour))
                        {
                            float gNew, hNew, fNew;

                            gNew = details[i, j].G + 1.0f;
                            hNew = CalculateHValue(neighbour, dest);
                            fNew = gNew + hNew;

                            if (details[neighbour.X, neighbour.Z].F == -1 || details[neighbour.X, neighbour.Z].F > fNew)
                            {
                                openList.Add(new Tuple<float, int, int>(fNew, neighbour.X, neighbour.Z));

                                details[neighbour.X, neighbour.Z].G = gNew;
                                details[neighbour.X, neighbour.Z].H = hNew;
                                details[neighbour.X, neighbour.Z].F = fNew;
                                details[neighbour.X, neighbour.Z].Parent = new(i, j);
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    private static Vec2 TracePath(Cell[,] details, Vec2 dest)
    {
        const int justInCase = 1000000;
        int i = 0;

        int row = dest.X;
        int col = dest.Z;

        var node = details[row, col];
        var nextNode = node.Parent;

        do
        {
            i++;

            node = details[row, col];
            nextNode = node.Parent;
            row = nextNode.X;
            col = nextNode.Z;

            if (i >= justInCase)
                break;

        } while (details[row, col].Parent != nextNode);

        return node.Original;
    }

    private static bool IsValid(bool[,] grid, Vec2 point)
    {
        return point.X >= 0 && point.X < grid.GetLength(0)
            && point.Z >= 0 && point.Z < grid.GetLength(1);
    }

    private static bool IsNotBlocked(bool[,] grid, Vec2 point)
    {
        return IsValid(grid, point) && grid[point.X, point.Z] == false;
    }

    private static float CalculateHValue(Vec2 src, Vec2 dest)
    {
        return Mathf.Sqrt(
            Mathf.Pow((src.X - dest.X), 2.0f) +
            Mathf.Pow((src.Z - dest.Z), 2.0f));
    }

    private static (int, int)? GetDestinationIndices(List<Vec2> positions, Vec2 dest, int size)
    {
        int i = 0;

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                if (positions[i] == dest)
                {
                    return (x, z);
                }

                i++;
            }
        }

        return null;
    }

    private class Cell
    {
        public Vec2 Original { get; set; }
        public Vec2 Parent { get; set; }
        public float F { get; set; }
        public float G { get; set; }
        public float H { get; set; }

        public Cell()
        {
            Parent = new Vec2(-1, -1);
            F = -1f;
            G = -1f;
            H = -1f;
        }
    }
}