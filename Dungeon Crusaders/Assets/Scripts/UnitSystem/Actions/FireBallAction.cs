using UnityEngine;

public class FireBallAction : UnitAction
{
    private const int ManaCost = 8;

    public FireBallAction()
    {
        SelectionType = SelectionTypes.Area;
        Name = "Fireball";
        _baseRange = 3;
    }

    public override void Execute(Unit unit, UnitManager context, params object[] args)
    {
        var centerTile = (Vec2)args[0];

        var unitsAroundCenter = context.FindUnitsAround(centerTile, 1);
        var centerUnit = context.UnitAt(centerTile);

        if (centerUnit != null) unitsAroundCenter.Add(centerUnit);

        var centerWorldPosition = new Vector3(centerTile.X + LevelManager.CenterOffset, 0f, centerTile.Z + LevelManager.CenterOffset);

        unit.transform.LookAt(centerWorldPosition);

        var unitStats = unit.ApplyBuffs();
        unitStats.CurrentMana -= ManaCost;
        unit.Stats = unitStats;

        var damage = EvaluateDamage(unit, unitStats);

        FireballEffect.Play(context, unit.transform.position, centerWorldPosition + new Vector3(0, 0.25f, 0));

        unit.PlayCast();
        context.TakeControlFor(1f, unit.PlayIdle);

        foreach (var u in unitsAroundCenter)
        {
            var unitTile = u.TilePosition;
            var (damageReceived, healthRemaining) = u.ReceiveAttack(damage, DamageType.Magical);

            NumberPopup.Create(
                u.transform.position + Vector3.up,
                damageReceived,
                Color.red);

            if (healthRemaining <= 0)
            {
                context.UnitDestroyed(unitTile);
            }
        }
        // TODO: play damage animation
    }

    public override bool IsUsable(Unit unit)
    {
        return unit.Stats.CurrentMana >= ManaCost;
    }

    public override int Area()
    {
        return 1;
    }

    public override int Range(Unit unit)
    {
        return _baseRange;
    }

    public override string Tooltip(Unit unit)
    {
        return $"Select an area within 3 tiles. Deals {EvaluateDamage(unit)} fire damage to all units in a 3x3 area.";
    }

    private int EvaluateDamage(Unit unit, UnitStats buffedStats = null)
    {
        if (buffedStats == null) buffedStats = unit.ApplyBuffs();

        return buffedStats.SpellPower();
    }
}