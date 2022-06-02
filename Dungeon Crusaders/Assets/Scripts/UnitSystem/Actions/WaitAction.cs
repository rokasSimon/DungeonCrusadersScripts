public class WaitAction : UnitAction
{
    public WaitAction()
    {
        SelectionType = SelectionTypes.Self;
        Name = "Wait";
        _baseRange = 0;
    }

    public override void Execute(Unit unit, UnitManager context, params object[] args)
    {
        return;
    }

    public override int Range(Unit unit)
    {
        return _baseRange;
    }

    public override string Tooltip(Unit unit) => "Skip turn.";
}
