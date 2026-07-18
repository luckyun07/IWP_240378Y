using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Xeomon
{
    [SerializeField] XeomonBaseInformation _base;
    [SerializeField] int _level;

    public Xeomon(XeomonBaseInformation Base, int level)
    {
        _base = Base;
        _level = level;

        Init();
    }

    public XeomonBaseInformation BaseInformation
    {
        get { 
            return _base; 
        }
    }
    public int Level {
        get { 
            return _level;
        }
    }

    public int Exp { get; set; }
    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    //public Condition Status { get; private set; }
    public int StatusTime { get; set; }
    //public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }

    public Queue<string> StatusChanges { get; private set; }
    public bool HpChanged { get; set; }
    public event System.Action OnStatusChanged;

    public void Init()
    {
        // Get moves
        Moves = new List<Move>();
        foreach (var move in BaseInformation.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.BaseInformation));
            if (Moves.Count >= XeomonBaseInformation.MaxNumOfMoves)
                break;
        }

        Exp = BaseInformation.GetExpForLevel(Level);

        CalculateStats();
        HP = MaxHP;

        StatusChanges = new Queue<string>();
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.SpAttack, 0 },
            { Stat.SpDefense, 0 },
            { Stat.Speed, 0},
        };
        //Status = null;
        //VolatileStatus= null;
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((BaseInformation.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((BaseInformation.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((BaseInformation.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((BaseInformation.SpDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((BaseInformation.Speed * Level) / 100f) + 5);

        MaxHP = Mathf.FloorToInt((BaseInformation.MaxHP * Level) / 100f) + 10;
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        // Apply stat stage changes here
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if(boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boostAmount;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            Debug.Log($"Boosted {stat} by {boost}.");
        }
    }

    public bool CheckForLevelUp()
    {
        if (Exp > _base.GetExpForLevel(Level + 1))
        {
            _level++;
            return true;
        }

        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrentLevel()
    {
        return BaseInformation.LearnableMoves.Where(x => x.Level == _level).FirstOrDefault();
    }

    public void LearnMove(LearnableMove moveToLearn)
    {
        if (Moves.Count > XeomonBaseInformation.MaxNumOfMoves)
            return;

        Moves.Add(new Move(moveToLearn.BaseInformation));
    }

    public int MaxHP
    {
        get; private set;
    }

    public int Attack
    {
        get { 
            return GetStat(Stat.Attack);
        }
    }

     public int Defense
    {
        get { 
            return GetStat(Stat.Defense); 
        }
    }
     public int SpAttack
    {
        get { 
            return GetStat(Stat.SpAttack); 
        }
    }
     public int SpDefense
    {
        get { 
            return GetStat(Stat.SpDefense); 
        }
    }
     public int Speed
    {
        get { 
            return GetStat(Stat.Speed); 
        }
    }

    public DamageDetails TakeDamage(Move move, Xeomon attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;

        float element = ElementChart.GetEffectiveness(move.BaseInformation.Element, BaseInformation.Element1) * 
                        ElementChart.GetEffectiveness(move.BaseInformation.Element, BaseInformation.Element2);

        var damageDetails = new DamageDetails()
        {
            Critical = critical,
            Element = element,
            Fainted = false
        };

        float attack = (move.BaseInformation.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.BaseInformation.Category == MoveCategory.Special) ? SpDefense : Defense;

        if (move.BaseInformation.Power > 0)
        {
            float modifiers = Random.Range(0.85f, 1f) * element * critical;
            float a = (2 * attacker.Level + 10) / 250f;
            float d = a * move.BaseInformation.Power * ((float)attack / defense) + 2;
            int damage = Mathf.FloorToInt(d * modifiers);

            HP -= damage;
        }
        
        if (HP <= 0){
            HP = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }

    public DamageDetails TakeAbilityDamage(float dmg, Xeomon attacker, int num)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;

        float element = ElementChart.GetEffectiveness(attacker.BaseInformation.Element1, BaseInformation.Element1) *
                        ElementChart.GetEffectiveness(attacker.BaseInformation.Element1, BaseInformation.Element2);

        var damageDetails = new DamageDetails()
        {
            Critical = critical,
            Element = element,
            Fainted = false
        };

        float attack;
        float defense;
        float enhance = 1f;

        if (num == 2 || num == 3)
        {
            enhance = 1.5f;
            attacker.HP -= Mathf.FloorToInt(attacker.MaxHP * 0.1f);
        }

        if (num == 0 || num == 2)
        {
            attack = attacker.Attack * enhance;
            defense = Defense;
        }
        else if (num == 1 || num == 3)
        {
            attack = attacker.SpAttack * enhance;
            defense = SpDefense;
        }
        else if (num == 4)
        {
            attack = ((attacker.SpAttack > attacker.Attack) ? attacker.SpAttack:attacker.Attack) * 2;
            defense = (attacker.SpAttack > attacker.Attack) ? SpDefense:Defense;
        }
        else
        {
            attack = (attacker.SpAttack > attacker.Attack) ? attacker.SpAttack : attacker.Attack;
            defense = (attacker.SpAttack > attacker.Attack) ? SpDefense : Defense;
        }

        float modifiers = Random.Range(0.85f, 1f) * element * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * dmg * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;

        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float Element { get; set; }
}