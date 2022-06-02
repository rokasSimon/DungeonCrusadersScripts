using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private UnitStatsDisplay StatsDisplay;
    [SerializeField] private LayerMask SelectionLayer;
    [SerializeField] private GameObject Selector;
    [SerializeField] private GameObject AdditionalSelector;
    [SerializeField] private float HeightOffset = 0.1f;

    UnitManager _unitManager;
    Vector3 _previousPosition;
    Material _selectorMaterial;
    List<GameObject> _additionalSelectors;

    Stack<GameObject> _areaTargetingSelectors;
    int? _targetedAreaSize;

    bool _mouse1Active;
    bool _mouse2Active;

    void Start()
    {
        _targetedAreaSize = null;
        _unitManager = FindObjectOfType<UnitManager>();
        _selectorMaterial = Selector.GetComponent<MeshRenderer>().material;
        _areaTargetingSelectors = new Stack<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_unitManager.InputAllowed)
        {
            HandleMouseInput();

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, SelectionLayer))
            {
                var centerOffset = LevelManager.CenterOffset;

                var tileHeight = LevelManager.LevelHeight + HeightOffset;
                var position = new Vector3(Mathf.FloorToInt(hit.point.x) + centerOffset, tileHeight, Mathf.FloorToInt(hit.point.z) + centerOffset);
                var tilePosition = new Vec2(position);

                if (_mouse1Active)
                {
                    ClickOnTile(tilePosition);
                }

                if (position != _previousPosition)
                {
                    Selector.SetActive(true);
                    DisplayUnitStats(position);

                    Selector.transform.position = position;
                    _previousPosition = position;

                    if (_targetedAreaSize != null)
                    {
                        PlaceAreaTargetingSelectors(tilePosition);
                    }
                }

                SetSelectorColor(_unitManager.TeamColorAt(tilePosition));
            }
            else
            {
                _previousPosition = Vector3.positiveInfinity;
                ClearAreaSelectors();

                StatsDisplay.gameObject.SetActive(false);
                Selector.SetActive(false);
            }
        }
    }

    private void DisplayUnitStats(Vector3 position)
    {
        var tilePos = new Vec2(position);
        var unit = _unitManager.UnitAt(tilePos);

        if (unit != null)
        {
            StatsDisplay.gameObject.SetActive(true);
            StatsDisplay.UpdateDisplay(unit);
        }
        else
        {
            StatsDisplay.gameObject.SetActive(false);
        }
    }

    private void PlaceAreaTargetingSelectors(Vec2 tilePosition)
    {
        var emptyPositionsAround = _unitManager.FindPlayableAreaAround(tilePosition, _targetedAreaSize.Value);

        while (_areaTargetingSelectors.Count > emptyPositionsAround.Count)
        {
            var ats = _areaTargetingSelectors.Pop();

            Destroy(ats);
        }

        while (_areaTargetingSelectors.Count < emptyPositionsAround.Count)
        {
            var newSelector = Instantiate(AdditionalSelector, Vector3.zero, transform.rotation, transform);
            newSelector.SetActive(true);

            _areaTargetingSelectors.Push(newSelector);
        }

        int i = 0;
        foreach (var sel in _areaTargetingSelectors)
        {
            sel.transform.position = WorldPositionFromVec2(emptyPositionsAround[i++]);
        }
    }

    public void ChangeSelector(UnitAction action)
    {
        ClearAdditionalSelectors();
        switch (action.SelectionType)
        {
            case SelectionTypes.Self: TargetSelf(); break;
            case SelectionTypes.RangeEmpty: TargetRangeEmpty(action); break;
            case SelectionTypes.RangeEnemy: TargetRangeEnemy(action); break;
            case SelectionTypes.Area: EnableAreaTargeting(action); break;
            default: return;
        }
    }

    public void ClearSelection()
    {
        ClearAdditionalSelectors();
    }

    void ClickOnTile(Vec2 tile)
    {
        if (_additionalSelectors != null)
        {
            foreach (var select in _additionalSelectors)
            {
                var selectTile = new Vec2(select.transform.position);

                if (selectTile == tile)
                {
                    _unitManager.CompleteUnitAction(selectTile);
                    ClearAdditionalSelectors();
                    return;
                }
            }
        }
        else if (_targetedAreaSize != null)
        {
            _unitManager.CompleteUnitAction(tile);
            ClearAdditionalSelectors();
            return;
        }
    }

    void HandleMouseInput()
    {
        _mouse1Active = Input.GetMouseButtonDown(0);
        _mouse2Active = Input.GetMouseButtonDown(1);

        if (_mouse2Active)
        {
            ClearAdditionalSelectors();
            _unitManager.DismissAction();
        }
    }

    void ClearAreaSelectors()
    {
        for (int i = 0; i < _areaTargetingSelectors.Count; i++)
        {
            var ats = _areaTargetingSelectors.Pop();

            Destroy(ats);
        }
    }

    void ClearAdditionalSelectors()
    {
        if (_additionalSelectors != null)
        {
            foreach (var ads in _additionalSelectors)
            {
                ads.SetActive(false);
                Destroy(ads);
            }

            _additionalSelectors = null;
        }

        ClearAreaSelectors();
        _targetedAreaSize = null;
    }

    Vector3 WorldPositionFromVec2(Vec2 tile)
    {
        var centerOffset = LevelManager.CenterOffset;
        var height = LevelManager.LevelHeight + HeightOffset;

        return new Vector3(tile.X + centerOffset, height, tile.Z + centerOffset);
    }

    void SetSelectorColor(Color color)
    {
        const string colorProperty = "_OutlineColor";

        _selectorMaterial.SetColor(colorProperty, color);
    }

    void TargetSelf()
    {
        var unit = _unitManager.ActiveUnit;
        _additionalSelectors = new List<GameObject>(1);

        var tilePosition = unit.TilePosition;
        var position = WorldPositionFromVec2(tilePosition);

        var selfSelector = Instantiate(AdditionalSelector, position, transform.rotation, transform);
        selfSelector.SetActive(true);

        _additionalSelectors.Add(selfSelector);
    }

    void TargetRangeEmpty(UnitAction action)
    {
        var unit = _unitManager.ActiveUnit;
        var range = action.Range(unit);
        _additionalSelectors = new List<GameObject>();

        List<Vec2> emptyPositionsNearby = _unitManager.FindEmptyPositionsAround(unit.TilePosition, range);

        foreach (var tile in emptyPositionsNearby)
        {
            var worldPosition = WorldPositionFromVec2(tile);
            var selectorObject = Instantiate(AdditionalSelector, worldPosition, transform.rotation, transform);

            selectorObject.SetActive(true);

            _additionalSelectors.Add(selectorObject);
        }
    }

    void TargetRangeEnemy(UnitAction action)
    {
        var unit = _unitManager.ActiveUnit;
        var range = action.Range(unit);
        _additionalSelectors = new List<GameObject>();

        List<Unit> units = _unitManager.FindUnitsAround(unit.TilePosition, range);

        foreach (var foundUnit in units)
        {
            if (foundUnit.Team != unit.Team)
            {
                var tile = new Vec2(foundUnit.transform.position);
                var worldPosition = WorldPositionFromVec2(tile);
                var selectorObject = Instantiate(AdditionalSelector, worldPosition, transform.rotation, transform);

                selectorObject.SetActive(true);

                _additionalSelectors.Add(selectorObject);
            }
        }
    }

    void EnableAreaTargeting(UnitAction action)
    {
        _targetedAreaSize = action.Area();
    }
}
