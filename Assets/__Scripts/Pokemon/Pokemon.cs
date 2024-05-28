using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;
using static Cinemachine.DocumentationSortingAttribute;

[CreateAssetMenu(fileName = "New Pokemon", menuName = "My Assets/New Pokemon")]
public class Pokemon : ScriptableObject
{
    [Header("Information")]
    public PokemonInfo info;

    [Header("Other Needed Info")]
    public float shinyScale = 1f;
    public AnimatorController controller;
    public Avatar avatar;

    private void OnValidate()
    {
        if (info.movesByLevel.Count > 0)
            for (int i = 0; i <= info.movesByLevel.Count - 1; i++)
            {
                if (info.movesByLevel[i].move == null)
                    info.movesByLevel[i].name = "Level " + (i + 1);
                else
                    info.movesByLevel[i].name = string.Format("Level {0} - {1}", i + 1, info.movesByLevel[i].move.moveName);
            }
        if (info.evolutionsByItem.Count > 0)
            for (int i = 0; i <= info.evolutionsByItem.Count - 1; i++)
                if (info.evolutionsByItem[i].evolutionItem != null && info.evolutionsByItem[i].evolvesTo != null)
                    info.evolutionsByItem[i].name = string.Format("{0} - {1}", info.evolutionsByItem[i].evolutionItem.name, info.evolutionsByItem[i].evolvesTo.info.pokemonName);
    }

    public GameObject GetModel(bool shiny, Gender gender)
    {
        if (shiny)
        {
            switch (gender)
            {
                case Gender.None:
                    return info.profile.models.myShinyMaleModel;
                case Gender.Male:
                    return info.profile.models.myShinyMaleModel;
                case Gender.Female:
                    return info.profile.models.myShinyFemaleModel;
                case Gender.Legendary:
                    return info.profile.models.myShinyFemaleModel;
                default:
                    return info.profile.models.myMaleModel;
            }
        }
        switch (gender)
        {
            case Gender.None:
                return info.profile.models.myMaleModel;
            case Gender.Male:
                return info.profile.models.myMaleModel;
            case Gender.Female:
                return info.profile.models.myFemaleModel;
            case Gender.Legendary:
                return info.profile.models.myFemaleModel;
            default:
                return info.profile.models.myMaleModel;

        }
    }

    public Stats CalculateStats(int level, Stats IV, Stats EV)
    {
        Stats returnedStats = new Stats();

        returnedStats.HP = (int)(Mathf.Floor(0.01f * (2f * info.baseStats.HP + IV.HP + Mathf.Floor(0.25f * EV.HP)) * level) + level + 10f);
        returnedStats.attack = (int)(Mathf.Floor((0.01f * (2f * info.baseStats.attack + IV.attack + Mathf.Floor(0.25f * EV.attack)) * level) + 5) * 1f);
        returnedStats.defense = (int)(Mathf.Floor((0.01f * (2f * info.baseStats.defense + IV.defense + Mathf.Floor(0.25f * EV.defense)) * level) + 5) * 1f);
        returnedStats.specialAttack = (int)(Mathf.Floor((0.01f * (2f * info.baseStats.specialAttack + IV.specialAttack + Mathf.Floor(0.25f * EV.specialAttack)) * level) + 5) * 1f);
        returnedStats.specialDefense = (int)(Mathf.Floor((0.01f * (2f * info.baseStats.specialDefense + IV.specialDefense + Mathf.Floor(0.25f * EV.specialDefense)) * level) + 5) * 1f);
        returnedStats.speed = (int)(Mathf.Floor((0.01f * (2f * info.baseStats.speed + IV.speed + Mathf.Floor(0.25f * EV.speed)) * level) + 5) * 1f);
        return returnedStats;
    }

    public bool ShinyCheck(int rolls = 1)
    {
        bool shine = false;
        for (int i = 0; i < rolls; i++)
        {
            int newRNG = Random.Range(0, 65536);
            if (newRNG <= 15)
                return true;
            else
                shine = false;
        }
        return shine;
    }

    public Gender AssignGender()
    {
        return (Gender)Random.Range(1, 3);
    }

    public void RecalculateStats(Owned owned)
    {
        owned.stats.HP = (int)(Mathf.Floor(0.01f * (2f * info.baseStats.HP + owned.IV.HP + Mathf.Floor(0.25f * owned.EV.HP)) * owned.level) + owned.level + 10f);
        owned.stats.attack = (int)(Mathf.Floor((0.01f * (2f * info.baseStats.attack + owned.IV.attack + Mathf.Floor(0.25f * owned.EV.attack)) * owned.level) + 5) * 1f);
        owned.stats.defense = (int)(Mathf.Floor((0.01f * (2f * info.baseStats.defense + owned.IV.defense + Mathf.Floor(0.25f * owned.EV.defense)) * owned.level) + 5) * 1f);
        owned.stats.specialAttack = (int)(Mathf.Floor((0.01f * (2f * info.baseStats.specialAttack + owned.IV.specialAttack + Mathf.Floor(0.25f * owned.EV.specialAttack)) * owned.level) + 5) * 1f);
        owned.stats.specialDefense = (int)(Mathf.Floor((0.01f * (2f * info.baseStats.specialDefense + owned.IV.specialDefense + Mathf.Floor(0.25f * owned.EV.specialDefense)) * owned.level) + 5) * 1f);
        owned.stats.speed = (int)(Mathf.Floor((0.01f * (2f * info.baseStats.speed + owned.IV.speed + Mathf.Floor(0.25f * owned.EV.speed)) * owned.level) + 5) * 1f);
        owned.nextLevelEXP = CalculateNextLevel(owned.level + 1, owned.pokemon.info.experienceRate) - CalculateNextLevel(owned.level, owned.pokemon.info.experienceRate);
        owned.EXPToNextLevel = owned.nextLevelEXP - owned.currentEXP;
        owned.myModel = owned.pokemon.GetModel(owned.shiny, owned.gender);
    }

    public int CalculateNextLevel(int nextLevel, ExperienceRate rate)
    {
        int exp = 100;
        switch (rate)
        {
            case ExperienceRate.Erratic:
                if (nextLevel < 50)
                {
                    exp = (int)(Mathf.Pow(nextLevel, 3) * (100 - nextLevel) / 50);
                }
                else if (nextLevel >= 50 && nextLevel < 68)
                {
                    exp = (int)(Mathf.Pow(nextLevel, 3) * (150 - nextLevel) / 100);
                }
                else if (nextLevel >= 68 && nextLevel < 98)
                {
                    exp = (int)(Mathf.Pow(nextLevel, 3) * Mathf.Floor((1911 - 10 * nextLevel) / 3) / 500);
                }
                else if (nextLevel >= 98)
                {
                    exp = (int)(Mathf.Pow(nextLevel, 3) * (160 - nextLevel) / 100);
                }
                break;

            case ExperienceRate.Fast:
                exp = (int)(4 * Mathf.Pow(nextLevel, 3) / 5);
                break;

            case ExperienceRate.MediumFast:
                exp = (int)Mathf.Pow(nextLevel, 3);
                break;

            case ExperienceRate.MediumSlow:
                exp = (int)(1.2f * Mathf.Pow(nextLevel, 3) - (15 * Mathf.Pow(nextLevel, 2)) + (100 * nextLevel) - 140);
                break;

            case ExperienceRate.Slow:
                exp = (int)(5 * Mathf.Pow(nextLevel, 3) / 4);
                break;

            case ExperienceRate.Fluctuating:
                if (nextLevel < 15)
                {
                    exp = (int)(Mathf.Pow(nextLevel, 3) * (Mathf.Floor((nextLevel + 1) / 3) + 24) / 50);
                }
                else if (nextLevel >= 15 && nextLevel < 36)
                {
                    exp = (int)(Mathf.Pow(nextLevel, 3) * (nextLevel + 14) / 50);
                }
                else if (nextLevel >= 36)
                {
                    exp = (int)(Mathf.Pow(nextLevel, 3) * (Mathf.Floor(nextLevel / 2) + 32) / 50);
                }
                break;
        }
        return exp;
    }

    public void LevelUp(Owned owned)
    {
        owned.level++;
        if (info.movesByLevel[owned.level - 1].move)
        {
            Move desiredMove = info.movesByLevel[owned.level - 1].move;
            bool knowsMove = false;
            for (int i = 0; i <= owned.moveset.Count - 1; i++)
            {
                if (owned.moveset[i].move == desiredMove)
                    knowsMove = true;
            }
            if (knowsMove)
            {
                Debug.Log(string.Format("{0} cannot learn {1} because it already knows {1}!", info.pokemonName, desiredMove));
            }
            else if (owned.moveset.Count - 1 < 3)//auto learn move 
            {
                LearnedMove newMove = new LearnedMove(desiredMove, 0);

                owned.moveset.Add(newMove);
                Debug.Log(string.Format("{0} learned {1}!", info.pokemonName, desiredMove));
            }
            else
            {
                //prompt to replace known move
                Debug.Log(string.Format("{0} cannot learn {1} at level {2} because it knows too many moves... right now.", info.pokemonName, desiredMove, owned.level));
            }
        }
        if (info.evolution != null)
            if (owned.level >= info.evolveLevel)
                owned.leveledUp = true;

        owned.currentEXP = 0;
        RecalculateStats(owned);
    }

    public int EvaluateEXPGain(Owned me, int ownedPokemonLevel)
    {
        float expGain;
        float b = info.experienceYield;
        float l = me.level;
        float s = 1f;
        float lp = ownedPokemonLevel;
        float t = 1f;
        float e = 1f;
        float v = 1f;
        float f = 1f;
        float p = 1f;

        expGain = (b * l / 5f * (1f / s) * Mathf.Pow(((2f * l) + 10f) / (l + lp + 10f), 2.5f) + 1f) * t * e * v * f * p;
        return (int)expGain;
    }

    public Nature AssignNature()
    {
        int RNG = Random.Range(0, 25);
        return (Nature)RNG;
    }

    public Stats RollIV()
    {
        Stats returnStats = new Stats();
        returnStats.HP = Random.Range(0, 32);
        returnStats.attack = Random.Range(0, 32);
        returnStats.defense = Random.Range(0, 32);
        returnStats.specialAttack = Random.Range(0, 32);
        returnStats.specialDefense = Random.Range(0, 32);
        returnStats.speed = Random.Range(0, 32);
        return returnStats;
    }
}
public enum Gender { None, Male, Female, Legendary, DittoSpecial }
public enum ConditionStatus { Healthy, Paralyzed, Burned, Asleep, Poisoned, BadlyPoisoned, Frozen, Fainted }

[System.Serializable]
public class ItemEvolve
{
    [HideInInspector]
    public string name;
    public EvolutionItem evolutionItem;
    public Pokemon evolvesTo;
};
