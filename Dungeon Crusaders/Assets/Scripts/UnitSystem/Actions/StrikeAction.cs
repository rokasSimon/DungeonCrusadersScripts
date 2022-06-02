using UnityEngine;

public class StrikeAction : UnitAction
{
    public StrikeAction()
    {
        SelectionType = SelectionTypes.RangeEnemy;
        Name = "Strike";
        _baseRange = 0;
    }

    public override void Execute(Unit unit, UnitManager context, params object[] args)
    {
        var unitTile = (Vec2)args[0];
        var enemy = context.UnitAt(unitTile);
        var enemyPositionWorld = enemy.transform.position;

        unit.transform.LookAt(enemyPositionWorld);
        var (damage, _) = EvaluateDamageAndCrit(unit);

        var (damageReceived, healthRemaining) = enemy.ReceiveAttack(damage, DamageType.Physical);

        NumberPopup.Create(
            new Vector3(enemyPositionWorld.x, enemyPositionWorld.y + 1f, enemyPositionWorld.z),
            damageReceived,
            Color.red);

        if (healthRemaining <= 0)
        {
            context.UnitDestroyed(unitTile);
        }
        // TODO: play damage animation
    }

    public override int Range(Unit unit)
    {
        var buffedStats = unit.ApplyBuffs();

        return buffedStats.AttackRange + _baseRange;
    }

    public override string Tooltip(Unit unit)
    {
        var (damage, critChance) = EvaluateDamageAndCrit(unit);

        return $"Attack a unit within {Range(unit)} tiles for {damage} damage. {critChance * 100} chance to critically strike for double damage.";
    }

    private (int, float) EvaluateDamageAndCrit(Unit unit)
    {
        var buffedStats = unit.ApplyBuffs();

        float randomNum = Random.value;
        int damage = buffedStats.AttackPower();

        if (randomNum < buffedStats.AttackCritChance())
        {
            damage *= 2;
        }

        return (damage, buffedStats.AttackCritChance());
    }
}