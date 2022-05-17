using UnityEngine;

public class LevelTileData
{
    public Vec2 Size { get; set; }
    public TileTypes[,] Tiles { get; set; }

    public TileTypes this[Vec2 idx]
    {
        get
        {
            return Tiles[idx.X, idx.Z];
        }
        set
        {
            Tiles[idx.X, idx.Z] = value;
        }
    }

    public bool Match(int x, int z, params TileTypes[] typesToMatch)
    {
        var t = Tiles[x, z];

        foreach (var type in typesToMatch)
        {
            if (t == type) return true;
        }

        return false;
    }
}
