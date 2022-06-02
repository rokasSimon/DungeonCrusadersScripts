using System.Collections;
using System.Collections.Generic;
using Constants;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] Vector3 Forward;
    [SerializeField] float WalkSpeed; // Animation walk speed

    public Vec2 TilePosition => new(transform.position);

    public string DisplayName;
    public string UnitName;
    public UnitTeam Team;

    public UnitStats Stats;
    public List<int> ActionIds;
    public List<int> StatusEffectIds;
    public List<UnitTag> UnitTags;

    Animator _animator;
    int _walkId;
    int _castId;

    AIController _aiController;

    public void Awake()
    {
        _aiController = GetComponent<AIController>();
        _animator = GetComponent<Animator>();
        _walkId = Animator.StringToHash("Speed");
        _castId = Animator.StringToHash("IsCasting");
    }

    public void Init()
    {
        if (ActionIds == null)
        {
            ActionIds = new List<int> {
                ActionRegister.Index<MoveAction>(),
                ActionRegister.Index<WaitAction>()
            };
        }
    }

    public UnitStats ApplyBuffs()
    {
        var newStats = Stats.Clone();

        foreach (var buffId in StatusEffectIds)
        {
            var buff = StatusRegister.Get(buffId);

            if (buff is IBuff b)
            {
                b.Modify(newStats);
            }
        }

        return newStats;
    }

    public void AIAction(UnitManager ctx)
    {
        _aiController.TakeTurn(ctx);
        //ctx.ExecuteAIAction(ActionRegister.Index<WaitAction>(), null);
    }

    public int EvaluateInitiative()
    {
        return 1;
    }

    public (int, int) ReceiveAttack(int incomingDamage, DamageType damageType)
    {
        var stats = ApplyBuffs();
        var damageInflicted = AppliedDamage(incomingDamage, damageType, stats);
        var remainingHealth = stats.CurrentHealth - damageInflicted;

        Stats.CurrentHealth = remainingHealth;

        if (remainingHealth <= 0)
        {
            //PlayDestruction();
        }

        return (damageInflicted, remainingHealth);
    }

    public void StartTurn()
    {
        if (Stats.CurrentMana < Stats.MaxMana())
        {
            Stats.CurrentMana++;
            NumberPopup.Create(transform.position + Vector3.up, 1, Color.blue);
        }
        //currentStats.StatusEffects.ForEach(effect => effect.OnStartTurn());
    }

    public void EndTurn()
    {
        foreach (var effectId in StatusEffectIds)
        {
            var effect = StatusRegister.Get(effectId);

            if (effect is IEndTurn e)
            {
                e.OnEndTurn(this);
            }
        }
    }

    private int AppliedDamage(int damage, DamageType type, UnitStats stats) => type switch
    {
        DamageType.Physical => damage - stats.Armor,
        DamageType.Magical => damage,
        _ => damage,
    };

    public void SetPlayerData(Player playerData)
    {
        DisplayName = playerData.Nickname;
        // Stats = playerData.Stats;
        UnitName = playerData.PrefabName;
        Team = UnitTeam.Player;
    }

    public void SmoothMove(Vector3 target, float timeToComplete)
    {
        StartCoroutine(InterpolateMovement(transform.position, target, timeToComplete));
    }

    IEnumerator InterpolateMovement(Vector3 start, Vector3 target, float timeToComplete)
    {
        float timeElapsed = 0;

        while (timeElapsed < timeToComplete)
        {
            transform.position = Vector3.Lerp(start, target, timeElapsed / timeToComplete);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    public void PlayWalk()
    {
        if (_animator != null)
        {
            _animator.SetFloat(_walkId, WalkSpeed);
        }
    }

    public void PlayIdle()
    {
        if (_animator != null)
        {
            _animator.SetFloat(_walkId, 0f);
            _animator.SetBool(_castId, false);
        }
    }

    public void PlayCast()
    {
        if (_animator != null)
        {
            _animator.SetBool(_castId, true);
        }
    }

    /*public void PlayDestruction()
    {
        if (_animator != null)
        {
            _animator.enabled = false;
        }

        shatter.ShatterGameObject();
    }*/
}
