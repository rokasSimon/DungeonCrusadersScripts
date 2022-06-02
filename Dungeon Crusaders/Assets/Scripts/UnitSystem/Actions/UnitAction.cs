public abstract class UnitAction
{
    protected int _baseRange;

    public string Name { get; protected set; }
    public SelectionTypes SelectionType { get; protected set; }

    //public abstract bool IsUsable(Unit unit);
    public virtual bool IsUsable(Unit unit)
    {
        return true;
    }

    public abstract void Execute(Unit unit, UnitManager context, params object[] args);
    public abstract int Range(Unit unit);

    public virtual int Area()
    {
        return 0;
    }

    public abstract string Tooltip(Unit unit);
}