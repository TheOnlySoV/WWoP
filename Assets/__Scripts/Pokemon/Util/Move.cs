using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

//[System.Serializable]
[CreateAssetMenu(fileName = "New Move", menuName = "My Assets/Move")]
public class Move : ScriptableObject
{
    public string moveName;
    public int powerPoints;
    public int basePower;
    public int accuracy;

    public Type battleType;
    public Category category;
    public string description;
    [Space(10)]
    public bool priorityMove;

    [Header("Status Condition Information")]
    public bool hasStatusCondition;
    public StatusCondition statusCondition;

    [Header("Stat Modifier Information")]
    public bool hasModifier;
    public Stats modifier;

    private void OnValidate()
    {
        moveName = name;
    }

    public (int, bool, float) CalculateDamage(Owned attacker, Owned target, Stats attackerStatModifiers, Stats targetStatModifiers)
    {
        float damage;
        float power = basePower;
        float A = attacker.stats.attack;
        float D = target.stats.defense;
        float targets = 1f;
        float pb = DetermineParentalBond();//parental bond
        float weather = DetermineWeatherEvaluation();
        float gr = DetermineGlaiveRush();//glaive rush
        float crit = CalculateCritical();
        float random = GetRandom();
        float stab;
        if (DetermineSTAB(attacker.pokemon.info.type1.type, battleType.type) || DetermineSTAB(attacker.pokemon.info.type2.type, battleType.type))
            stab = 1.5f;
        else
            stab = 1f;
        float type = battleType.CheckEffectiveness(battleType, target.pokemon);
        float burn = DetermineStatusEffect(attacker);
        float other = DetermineOther();

        switch ((int)category)
        {
            case 0:
                A = attacker.stats.attack * CalculateStatModifer(attackerStatModifiers.attack);
                D = target.stats.defense * CalculateStatModifer(targetStatModifiers.defense);
                break;
            case 1:
                A = attacker.stats.specialAttack * CalculateStatModifer(attackerStatModifiers.specialAttack);
                D = target.stats.specialDefense * CalculateStatModifer(attackerStatModifiers.specialDefense);
                break;
            case 2:
                A = 0;
                D = 0;
                break;
        }

        damage = ((((2f * attacker.level / 5f) + 2f) * power * (A / D) / 50f) + 2f) * targets * weather * pb * gr * crit * random * stab * type * burn * other;

        if (category == Category.Other)
        {
            damage = 0;
        }
        if (hasStatusCondition)
            return ((int)damage, statusCondition.EvaluateChance(), type);
        else
            return ((int)damage, false, type);
    }

    public (Stats, List<bool>) ApplyStatModifiers(Stats targetStatsToModify)
    {
        Stats modifiedStats = new Stats();
        List<bool> success = new List<bool>();

        int cachedValue = targetStatsToModify.HP + modifier.HP;
        if (cachedValue <= 6 && cachedValue >= -6)
        {
            modifiedStats.HP = targetStatsToModify.HP + modifier.HP;
            success.Add(true);
        } 
        else
        {
            modifiedStats.HP = targetStatsToModify.HP;
            success.Add(false);
        }

        cachedValue = targetStatsToModify.attack + modifier.attack;
        if (cachedValue <= 6 && cachedValue >= -6)
        {
            modifiedStats.attack = targetStatsToModify.attack + modifier.attack;
            success.Add(true);
        }
        else
        {
            modifiedStats.attack = targetStatsToModify.attack;
            success.Add(false);
        }

        cachedValue = targetStatsToModify.defense + modifier.defense;
        if (cachedValue <= 6 && cachedValue >= -6)
        {
            modifiedStats.defense = targetStatsToModify.defense + modifier.defense;
            success.Add(true);
        }
        else
        {
            modifiedStats.defense = targetStatsToModify.defense;
            success.Add(false);
        }

        cachedValue = targetStatsToModify.specialAttack + modifier.specialAttack;
        if (cachedValue <= 6 && cachedValue >= -6)
        {
            modifiedStats.specialAttack = targetStatsToModify.specialAttack + modifier.specialAttack;
            success.Add(true);
        }
        else
        {
            modifiedStats.specialAttack = targetStatsToModify.specialAttack;
            success.Add(false);
        }

        cachedValue = targetStatsToModify.specialDefense + modifier.specialDefense;
        if (cachedValue <= 6 && cachedValue >= -6)
        {
            modifiedStats.specialDefense = targetStatsToModify.specialDefense + modifier.specialDefense;
            success.Add(true);
        }
        else
        {
            modifiedStats.specialDefense = targetStatsToModify.specialDefense;
            success.Add(false);
        }

        cachedValue = targetStatsToModify.speed + modifier.speed;
        if (cachedValue <= 6 && cachedValue >= -6)
        {
            modifiedStats.speed = targetStatsToModify.speed + modifier.speed;
            success.Add(true);
        }
        else
        {
            modifiedStats.speed = targetStatsToModify.speed;
            success.Add(false);
        }

        return (modifiedStats, success);
    }

    float CalculateStatModifer(float statsToCheck)
    {
        float valueToReturn = 1f;
        switch (statsToCheck)
        {
            case -6:
                valueToReturn = 2f / 8f;
                break;
            case -5:
                valueToReturn = 2f / 7f;
                break;
            case -4:
                valueToReturn = 2f / 6f;
                break;
            case -3:
                valueToReturn = 2f / 5f;
                break;
            case -2:
                valueToReturn = 2f / 4f;
                break;
            case -1:
                valueToReturn = 2f / 3f;
                break;

            case 0:
                valueToReturn = 2f / 2f;
                break;

            case 1:
                valueToReturn = 3f / 2f;
                break;
            case 2:
                valueToReturn = 4f / 2f;
                break;
            case 3:
                valueToReturn = 5f / 2f;
                break;
            case 4:
                valueToReturn = 6f / 2f;
                break;
            case 5:
                valueToReturn = 7f / 2f;
                break;
            case 6:
                valueToReturn = 8f / 2f;
                break;

            default:
                valueToReturn = 1f;
                break;
        }

        return valueToReturn;
    }

    float CalculateCritical()
    {
        float rand1 = Random.Range(0, 50) / 10f;
        float rand2 = Random.Range(0, 50) / 10f;
        if (rand1 == rand2)
            return 1.5f;
        else
            return 1f;
    }

    float GetRandom()
    {
        return Random.Range(85, 101) / 100f;
    }

    bool DetermineSTAB(CritterType type1, CritterType type2)
    {
        if (type1 == type2)
            return true;
        else
            return false;
    }

    float DetermineOther()
    {
        return 1f;
    }

    float DetermineParentalBond()
    {
        return 1f;
    }

    float DetermineGlaiveRush()
    {
        return 1f;
    }

    float DetermineStatusEffect(Owned attacker)
    {
        if (attacker.status == ConditionStatus.Burned && category == Category.Physical)
            return .5f;
        return 1f;
    }

    float DetermineWeatherEvaluation()
    {
        return 1f;
    }
}
public enum Category { Physical, Special, Other }

[System.Serializable]
public class StatusCondition
{
    public ConditionStatus conditionStatus = ConditionStatus.Healthy;
    [Tooltip("Chance the status condition will be applied")]
    [Range(0, 100)]
    public int chance;

    public bool EvaluateChance()
    {
        int RNG = Random.Range(1, 101);

        if (RNG <= chance)
            return true;
        return false;

    }
}