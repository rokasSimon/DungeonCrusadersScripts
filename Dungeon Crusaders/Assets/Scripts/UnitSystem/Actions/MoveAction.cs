using UnityEngine;

public class MoveAction : UnitAction
{
    public MoveAction()
    {
        SelectionType = SelectionTypes.RangeEmpty;
        Name = "Move";
        _baseRange = 1;
    }

    public override void Execute(Unit unit, UnitManager context, params object[] args)
    {
        var tileToMoveTo = (Vec2)args[0];
        var unitHeight = unit.transform.position.y;
        var oldUnitTile = new Vec2(unit.transform.position);
        var newUnitPosition = new Vector3(tileToMoveTo.X + LevelManager.CenterOffset, 0f, tileToMoveTo.Z + LevelManager.CenterOffset);

        unit.PlayWalk();
        float moveDuration = 2f;

        unit.SmoothMove(newUnitPosition, moveDuration);
        context.TakeControlFor(moveDuration, unit.PlayIdle);

        unit.transform.LookAt(newUnitPosition);

        context.UnitMoved(oldUnitTile, new(newUnitPosition));
    }

    public override int Range(Unit unit)
    {
        // TODO: Scale with movement buffs?
        return _baseRange;
    }

    public override string Tooltip(Unit unit)
    {
        return $"Move within {Range(unit)} nearby tiles.";
    }
}