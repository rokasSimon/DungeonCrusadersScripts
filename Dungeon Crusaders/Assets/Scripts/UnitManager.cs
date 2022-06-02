using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Constants;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

[RequireComponent(typeof(LevelExitController))]
public class UnitManager : MonoBehaviour
{
    [SerializeField] private GameObject UnitTemplate;
    [SerializeField] private UnitUIManager UnitUIManager;
    [SerializeField] private SelectionManager SelectionManager;
    [SerializeField] private LevelExitController LevelExitController;
    [SerializeField] private bool RunAI = false;

    [SerializeField] private List<GameObject> SpawnableEnemies;

    [NonSerialized] public bool LevelEnded;
    public bool InputAllowed => _inputAllowed;
    public Unit ActiveUnit => _initiativeQueue?.Peek();

    bool _inputAllowed;
    int _activeActionId;
    Queue<Unit> _initiativeQueue;
    UnitLocationController _grid;
    private Player _playerData;

    private void Awake()
    {
        LevelExitController = GetComponent<LevelExitController>();
    }

    void Update()
    {
        if (LevelEnded)
            return;
        
        if (_initiativeQueue is {Count: 0})
        {
            NewTurn();
        }

        if (RunAI && _inputAllowed)
        {
            var activeUnit = ActiveUnit;

            if (activeUnit != null && activeUnit.Team == UnitTeam.AI)
            {
                activeUnit.AIAction(this);
            }
        }
    }

    public void Init(LevelTileData levelTileData, int level)
    {
        if (_grid != null)
        {
            foreach (var unit in _grid.Units)
            {
                Destroy(unit.gameObject);
            }
        }

        _grid = new UnitLocationController(levelTileData);

        LoadPlayerUnits();
        PlaceEnemyUnits(level);

        LevelExitController.Init(_grid);

        LevelEnded = false;
        _inputAllowed = false;
    }

    public void TakeControlFor(float seconds, Action afterEffect)
    {
        StartCoroutine(TakeControl(seconds, afterEffect));
    }

    public void StartGame()
    {
        // TODO: play some animations, then start turn and allow controls
        NewTurn();
        _inputAllowed = true;
    }

    public Color TeamColorAt(Vec2 v)
    {
        if (_grid.UnitAt(v) != null)
        {
            return _grid.UnitAt(v).Team switch
            {
                UnitTeam.Player => TileColors.Player,
                UnitTeam.AI => TileColors.AI,
                UnitTeam.Other => TileColors.Other,
                _ => TileColors.Unknown,
            };
        }

        return TileColors.Default;
    }

    public Unit UnitAt(Vec2 pos)
    {
        return _grid.UnitAt(pos);
    }

    public void ClickOnActionButton(int actionNumber)
    {
        var unit = _initiativeQueue.Peek();
        _activeActionId = unit.ActionIds[actionNumber];
        var activeAction = ActionRegister.Get(_activeActionId);

        UnitUIManager.DisplayTooltip(unit, activeAction);
        SelectionManager.ChangeSelector(activeAction);
    }

    public void ClickOnCompleteButton()
    {
        if (_activeActionId != -1 && ActionRegister.Get(_activeActionId).SelectionType == SelectionTypes.Self)
        {
            SelectionManager.ClearSelection();
            CompleteUnitAction(null);
        }
    }

    public void CompleteUnitAction(params object[] args)
    {
        ActionRegister.Get(_activeActionId).Execute(ActiveUnit, this, args);

        if (_initiativeQueue.Count != 0)
        {
            _initiativeQueue.Dequeue();
        }

        DismissAction();

        LevelExitController.CheckForHeroInRange();

        if (_initiativeQueue.Count > 0)
        {
            UnitUIManager.DisplayActiveUnit(ActiveUnit);
        }
    }

    public void ExecuteAIAction(int actionId, params object[] args)
    {
        _activeActionId = actionId;
        CompleteUnitAction(args);
    }

    public void DismissAction()
    {
        _activeActionId = -1;
        UnitUIManager.HideTooltip();
    }

    public void UnitMoved(Vec2 prevTile, Vec2 newTile)
    {
        _grid.MoveUnit(prevTile, newTile);
    }

    public void UnitDestroyed(Vec2 pos)
    {
        var unit = _grid.UnitAt(pos);
        Destroy(unit.gameObject, 1);

        _grid.RemoveUnit(pos);
        LevelExitController.CheckForLevelFinish();
        ReorderQueue(pos);
    }

    public bool IsPathable(Vec2 v)
    {
        return _grid.IsPathable(v);
    }

    public List<Vec2> FindEmptyPositionsAround(Vec2 tilePosition, int range)
    {
        var positions = tilePosition.Range(range);
        var emptyTiles = new List<Vec2>();

        foreach (var position in positions)
        {
            if (IsPathable(position))
            {
                emptyTiles.Add(position);
            }
        }

        return emptyTiles;
    }

    public List<Unit> FindUnitsAround(Vec2 tilePosition, int range)
    {
        var positions = tilePosition.Range(range);
        var tiles = new List<Unit>();

        foreach (var position in positions)
        {
            var unit = _grid.UnitAt(position);

            if (unit != null)
            {
                tiles.Add(unit);
            }
        }

        return tiles;
    }

    public List<Unit> FindUnitAroundCircle(Vec2 tilePosition, int range)
    {
        var positions = tilePosition.RangeOuter(range);
        var tiles = new List<Unit>();

        foreach (var position in positions)
        {
            var unit = _grid.UnitAt(position);

            if (unit != null)
            {
                tiles.Add(unit);
            }
        }

        return tiles;
    }

    public List<Vec2> FindPlayableAreaAround(Vec2 tilePosition, int range)
    {
        var positions = tilePosition.Range(range);
        var emptyTiles = new List<Vec2>();

        foreach (var position in positions)
        {
            if (_grid.IsPlayArea(position))
            {
                emptyTiles.Add(position);
            }
        }

        return emptyTiles;
    }

    public bool[,] FindInBoundsArea(Vec2 tilePosition, int range)
    {
        var (xSize, zSize) = _grid.LevelSize;

        var botX = Mathf.Max(0, tilePosition.X - range);
        var botZ = Mathf.Max(0, tilePosition.Z - range);

        var topX = Mathf.Min(xSize - 1, tilePosition.X + range);
        var topZ = Mathf.Min(zSize - 1, tilePosition.Z + range);

        var grid = new bool[topX - botX + 1, topZ - botZ + 1];

        // TODO

        return grid;
    }

    public (List<Vec2>, bool[,]) GetRawPathing(Vec2 tilePosition, int range)
    {
        var positions = tilePosition.RangeFull(range);
        var size = range * 2 + 1;
        var blockedTiles = new bool[size, size];

        int i = 0;

        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                blockedTiles[x, z] = !_grid.IsPathable(positions[i]);

                i++;
            }
        }

        return (positions, blockedTiles);
    }

    public List<Vec2> FindEmptyAreaCircle(Vec2 tilePosition, int range)
    {
        var positions = tilePosition.RangeOuter(range);
        var emptyTiles = new List<Vec2>();

        foreach (var position in positions)
        {
            if (_grid.IsPathable(position))
            {
                emptyTiles.Add(position);
            }
        }

        return emptyTiles;
    }

    private void NewTurn()
    {
        foreach (var unit in _grid.Units)
        {
            unit.StartTurn();
        }

        ReorderQueue();

        UnitUIManager.DisplayActiveUnit(ActiveUnit);
    }

    private void ReorderQueue(Vec2? unitAtPositionToExclude = null)
    {
        if (_initiativeQueue == null || _initiativeQueue.Count == 0)
        {
            _initiativeQueue = new Queue<Unit>(_grid.Units.OrderByDescending(u => u.EvaluateInitiative()));
        }
        else
        {
            _initiativeQueue = new Queue<Unit>(_initiativeQueue
                .Where(u => unitAtPositionToExclude == null || u.TilePosition == unitAtPositionToExclude)
                .OrderByDescending(u => u.EvaluateInitiative()));
            //.SkipWhile(u => unitAtPositionToExclude != null && u.TilePosition == unitAtPositionToExclude.Value));

            foreach (var item in _initiativeQueue)
            {
                Debug.Log($"{item.DisplayName}: {item.TilePosition}");
            }
        }
    }

    private void PlaceEnemyUnits(int level)
    {
        var numberOfUnitsToGenerate = (int)Mathf.Log(4 * level);
        var exitPosition = _grid.FirstOrDefault(TileTypes.Exit);

        if (exitPosition == null)
        {
            throw new NullReferenceException("Couldn't find exit");
        }

        for (int i = 0; i < numberOfUnitsToGenerate; i++)
        {
            var randomUnitType = SpawnableEnemies.TakeRandom();

            int j = 1;
            var positions = FindEmptyPositionsAround(exitPosition.Value, j);

            while (positions.Count == 0)
            {
                if (j > _grid.LevelSize.X || j > _grid.LevelSize.Z)
                {
                    return;
                }

                j++;
                positions = FindEmptyPositionsAround(exitPosition.Value, j);
            }
            
            var unitSpawnPosition = positions.TakeRandom();

            var worldPos = new Vector3(unitSpawnPosition.X + LevelManager.CenterOffset, LevelManager.LevelHeight, unitSpawnPosition.Z + LevelManager.CenterOffset);
            var unitInstance = Instantiate(randomUnitType, worldPos, RandomRotation.Quaternion, transform);
            var unitComponent = unitInstance.GetComponent<Unit>();

            _grid.AddUnit(unitComponent, unitSpawnPosition);
        }
    }

    public void SavePlayerUnits()
    {
        Repository.SavePlayerData(_playerData);
    }

    private void LoadPlayerUnits()
    {
        _playerData = Repository.LoadPlayerData();

        if (_playerData is null)
        {
            throw new NullReferenceException("Player data doesn't exist");
        }

        var units = _playerData.Units;

        var entrancePosition = _grid.FirstOrDefault(TileTypes.Entrance);
        if (entrancePosition == null)
        {
            throw new NullReferenceException("Couldn't find entrance");
        }

        var positions = FindEmptyPositionsAround(entrancePosition.Value, 1);
        var pos = positions[UnityEngine.Random.Range(0, positions.Count)];

        var worldPos = new Vector3(pos.X + LevelManager.CenterOffset, LevelManager.LevelHeight,
            pos.Z + LevelManager.CenterOffset);
        var heroPrefab = Resources.Load<GameObject>(_playerData.PrefabName);

        var unitInstance = Instantiate(heroPrefab, worldPos, RandomRotation.Quaternion * heroPrefab.transform.rotation,
            transform);
        var unitComponent = unitInstance.GetComponent<Unit>();

        _grid.AddUnit(unitComponent, pos);
    }

    private IEnumerator TakeControl(float seconds, Action afterEffect)
    {
        _inputAllowed = false;

        yield return new WaitForSeconds(seconds);

        afterEffect();
        _inputAllowed = true;
    }
}