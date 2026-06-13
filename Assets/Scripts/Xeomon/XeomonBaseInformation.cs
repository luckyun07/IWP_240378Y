using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Xeomon", menuName = "Xeomon/Create new xeomon")]

public class XeomonBaseInformation : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] XeomonElement element1;
    [SerializeField] XeomonElement element2;

    // Base stats
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] int catchRate = 255;

    [SerializeField] List<LearnableMove> learnableMoves;
    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }

    public XeomonElement Element1
    {
        get { return element1; }
    }

    public XeomonElement Element2
    {
        get { return element2; }
    }

    public int MaxHP
    {
        get { return maxHP; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int SpAttack
    {
        get { return spAttack; }
    }

    public int SpDefense
    {
        get { return spDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }

    public int CatchRate => catchRate;
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBaseInformation baseInformation;
    [SerializeField] int level;

    public MoveBaseInformation BaseInformation
    {
        get { return baseInformation; }
    }

    public int Level
    {
        get { return level; }
    }
}

public enum XeomonElement
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed
}

public class ElementChart
{
    static float[][] chart =
    {
        /*NOR*/ new float[] {1f,1f,1f,1f,1f,1f,1f,1f,1f,1f,1f,1f,0.5f,0f,1f,1f,0.5f,1f},

        /*FIR*/ new float[] {1f,0.5f,0.5f,1f,2f,2f,1f,1f,1f,1f,1f,2f,0.5f,1f,0.5f,1f,2f,1f},

        /*WAT*/ new float[] {1f,2f,0.5f,1f,0.5f,1f,1f,1f,2f,1f,1f,1f,2f,1f,0.5f,1f,1f,1f},

        /*ELE*/ new float[] {1f,1f,2f,0.5f,0.5f,1f,1f,1f,0f,2f,1f,1f,1f,1f,0.5f,1f,1f,1f},

        /*GRA*/ new float[] {1f,0.5f,2f,1f,0.5f,1f,1f,0.5f,2f,0.5f,1f,0.5f,2f,1f,0.5f,1f,0.5f,1f},

        /*ICE*/ new float[] {1f,0.5f,0.5f,1f,2f,0.5f,1f,1f,2f,2f,1f,1f,1f,1f,2f,1f,0.5f,1f},

        /*FIG*/ new float[] {2f,1f,1f,1f,1f,2f,1f,0.5f,1f,0.5f,0.5f,0.5f,2f,0f,1f,2f,2f,0.5f},

        /*POI*/ new float[] {1f,1f,1f,1f,2f,1f,1f,0.5f,0.5f,1f,1f,1f,0.5f,0.5f,1f,1f,0f,2f},

        /*GRO*/ new float[] {1f,2f,1f,2f,0.5f,1f,1f,2f,1f,0f,1f,0.5f,2f,1f,1f,1f,2f,1f},

        /*FLY*/ new float[] {1f,1f,1f,0.5f,2f,1f,2f,1f,1f,1f,1f,2f,0.5f,1f,1f,1f,0.5f,1f},

        /*PSY*/ new float[] {1f,1f,1f,1f,1f,1f,2f,2f,1f,1f,0.5f,1f,1f,1f,1f,0f,0.5f,1f},

        /*BUG*/ new float[] {1f,0.5f,1f,1f,2f,1f,0.5f,0.5f,1f,0.5f,2f,1f,1f,0.5f,1f,2f,0.5f,0.5f},

        /*ROC*/ new float[] {1f,2f,1f,1f,1f,2f,0.5f,1f,0.5f,2f,1f,2f,1f,1f,1f,1f,0.5f,1f},

        /*GHO*/ new float[] {0f,1f,1f,1f,1f,1f,1f,1f,1f,1f,2f,1f,1f,2f,1f,0.5f,1f,1f},

        /*DRA*/ new float[] {1f,1f,1f,1f,1f,1f,1f,1f,1f,1f,1f,1f,1f,1f,2f,1f,0.5f,0f},

        /*DAR*/ new float[] {1f,1f,1f,1f,1f,1f,0.5f,1f,1f,1f,2f,1f,1f,2f,1f,0.5f,1f,0.5f},

        /*STE*/ new float[] {1f,0.5f,0.5f,0.5f,1f,2f,1f,1f,1f,1f,1f,1f,2f,1f,1f,1f,0.5f,2f},

        /*FAI*/ new float[] {1f,0.5f,1f,1f,1f,1f,2f,0.5f,1f,1f,1f,1f,1f,1f,2f,2f,0.5f,1f}
    };

    public static float GetEffectiveness(XeomonElement attackElement, XeomonElement defenseElement)
    {
        if (attackElement == XeomonElement.None || defenseElement == XeomonElement.None)
            return 1f;
        int row = (int)attackElement - 1;
        int col = (int)defenseElement - 1;
        return chart[row][col];
    }
}