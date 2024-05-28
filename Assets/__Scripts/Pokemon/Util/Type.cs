using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Type", menuName = "My Assets/Pokemon Type")]
public class Type : ScriptableObject
{
    public string typeName;
    public CritterType type;
    public Color typeColor = new Color(0f, 0f, 0f, 1f);
    public List<Sprite> typeSprites;

    [Header("Battle Lists")]
    public List<Type> superEffectiveAgainst = new List<Type>();
    public List<Type> notVeryEffectiveAgainst = new List<Type>();
    public List<Type> neutralAgainst = new List<Type>();
    public List<Type> noEffect = new List<Type>();

    public bool fixNeutralList = false;

    private void OnValidate()
    {
        if (fixNeutralList)
        {
            CheckAgainst(superEffectiveAgainst);
            CheckAgainst(notVeryEffectiveAgainst);
            CheckAgainst(noEffect);
            fixNeutralList = false;
        }
    }

    void CheckAgainst(List<Type> list)
    {
        foreach (Type t in list)
        {
            if (neutralAgainst.Contains(t))
                neutralAgainst.Remove(t);
        }
    }

    public float CheckEffectiveness(Type attackingType, Pokemon defendingPokemon)
    {
        float effectivenessValue = 1f;
        effectivenessValue *= CheckList(attackingType.superEffectiveAgainst, defendingPokemon.info.type1, 2f);
        effectivenessValue *= CheckList(attackingType.neutralAgainst, defendingPokemon.info.type1, 1f);
        effectivenessValue *= CheckList(attackingType.notVeryEffectiveAgainst, defendingPokemon.info.type1, .5f);
        effectivenessValue *= CheckList(attackingType.noEffect, defendingPokemon.info.type1, 0f);
        if (defendingPokemon.info.type2.typeName != "None")
        {
            effectivenessValue *= CheckList(attackingType.superEffectiveAgainst, defendingPokemon.info.type2, 2f);
            effectivenessValue *= CheckList(attackingType.neutralAgainst, defendingPokemon.info.type2, 1f);
            effectivenessValue *= CheckList(attackingType.notVeryEffectiveAgainst, defendingPokemon.info.type2, .5f);
            effectivenessValue *= CheckList(attackingType.noEffect, defendingPokemon.info.type2, 0f);
        }

        return effectivenessValue;
    }

    float CheckList(List<Type> attackingTypeList, Type defendingType, float valueMultiplier)
    {
        float value = 1f;
        if (attackingTypeList.Contains(defendingType))
            value *= valueMultiplier;
        return value;
    }
}
public enum CritterType { Normal, Fire, Water, Electric, Grass, Ice, Fighting, Poison, Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon, Dark, Steel, Fairy, None }