using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    public static float LevelHeight { get; private set; }
    public static float CenterOffset { get; private set; }

    [SerializeField] float SelectionGridOffset = 0.1f;

    [Header("Width")]
    [Min(2), SerializeField] int MinWidth = 2;
    [Min(2), SerializeField] int MaxWidth = 50;

    [Header("Length")]
    [Min(2), SerializeField] int MinLength = 2;
    [Min(2), SerializeField] int MaxLength = 50;

    [Header("Special tiles")]
    [Tooltip("Number of special tiles will be equal to square root of this value.")]
    [Min(4), SerializeField] int minimumSizeForExtras = 9;
    [SerializeField] TileTypes[] specialTileTypes;

    [SerializeField] GameObject FloorGrid;
    [SerializeField] GameObject SelectionGrid;
    [SerializeField] GameObject EnvironmentMap;
    [SerializeField] public GameObject UnitManager;

    public int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = 0;
        NewLevel();
    }

    public void NewLevel()
    {
        currentLevel++;

        var levelData = GenerateLevel();
        var selectionGrid = SelectionGrid.GetComponent<TileMap>();
        var floorGrid = FloorGrid.GetComponent<TileMap>();
        var objectMapComponent = EnvironmentMap.GetComponent<EnvironmentMap>();
        var unitManagerComponent = UnitManager.GetComponent<UnitManager>();

        var tilePlacementForSelectionGrid = GenerateTilePlacementArray(levelData, TileTypes.Empty, TileTypes.ReservedTile);
        var tilePlacementForFloorGrid = GenerateTilePlacementArray(levelData, TileTypes.Empty, TileTypes.ReservedTile, TileTypes.Column);

        selectionGrid.BuildMesh(tilePlacementForSelectionGrid, LevelHeight + SelectionGridOffset);
        floorGrid.BuildMesh(tilePlacementForFloorGrid, LevelHeight);
        objectMapComponent.PlaceObjects(levelData);
        unitManagerComponent.Init(levelData, currentLevel);
        unitManagerComponent.StartGame();
    }

    LevelTileData GenerateLevel()
    {
        var level = new LevelTileData();
        LevelHeight = 0;
        CenterOffset = 0.5f;

        // New level dimensions
        int x = Random.Range(MinLength, MaxLength);
        int z = Random.Range(MinWidth, MaxWidth);

        level.Size = new(x, z);

        level.Tiles = GenerateTileGrid(level.Size);

        return level;
    }

    TileTypes[,] GenerateTileGrid(Vec2 v)
    {
        var tiles = new TileTypes[v.X, v.Z];

        var entrance = new Vec2(Random.Range(0, v.X), Random.Range(0, v.Z));
        tiles[entrance.X, entrance.Z] = TileTypes.Entrance;

        var exit = GenerateExitPosition(v, entrance);
        tiles[exit.X, exit.Z] = TileTypes.Exit;

        MarkRequiredPath(tiles, entrance, exit);

        // Generate pits and columns only if map is sufficiently big
        if (v.X * v.Z > minimumSizeForExtras)
        {
            MarkSpecialTiles(tiles, v);
        }

        return tiles;
    }

    Vec2 GenerateExitPosition(Vec2 v, Vec2 entrance)
    {
        Vec2 exit;

        do
        {
            exit = new(Random.Range(0, v.X), Random.Range(0, v.Z));
        } while (exit == entrance);

        return exit;
    }

    void MarkRequiredPath(TileTypes[,] tiles, Vec2 entrance, Vec2 exit)
    {
        Vector2 current = entrance;
        Vector2 next = exit;

        while (current != exit)
        {
            var heading = next - current;
            var direction = heading / heading.magnitude;

            int x = Mathf.RoundToInt(current.x + direction.x);
            int z = Mathf.RoundToInt(current.y + direction.y);

            if (tiles[x, z] == TileTypes.Exit)
            {
                return;
            }

            tiles[x, z] = TileTypes.ReservedTile;

            current.x = x;
            current.y = z;
        }
    }

    void MarkSpecialTiles(TileTypes[,] tiles, Vec2 v)
    {
        int specialTileCount = Mathf.FloorToInt(Mathf.Sqrt(minimumSizeForExtras));

        while (specialTileCount > 0)
        {
            Vec2 rand = new(Random.Range(0, v.X), Random.Range(0, v.Z));

            if (tiles[rand.X, rand.Z] == TileTypes.Empty)
            {
                int randomIndex = Random.Range(0, specialTileTypes.Length);
                
                tiles[rand.X, rand.Z] = specialTileTypes[randomIndex];
                specialTileCount--;
            }
        }
    }

    bool[,] GenerateTilePlacementArray(LevelTileData levelData, params TileTypes[] typesToMatch)
    {
        var (x, z) = levelData.Size;
        var tilePositions = new bool[x, z];

        for (int xi = 0; xi < x; xi++)
        {
            for (int zi = 0; zi < z; zi++)
            {
                tilePositions[xi, zi] = levelData.Match(xi, zi, typesToMatch);
            }
        }

        return tilePositions;
    }
}
