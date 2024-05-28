using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public int HP;
    public int attack;
    public int defense;
    public int specialAttack;
    public int specialDefense;
    public int speed;
}
[System.Serializable]
public class PokemonInfo
{
    public int pID;
    public string pokemonName;

    [Space(10)]

    public Gender lockedGender = Gender.None;
    public Type type1;
    public Type type2;

    [Space(10)]

    public List<Ability> abilities;
    public Ability hiddenAbility;

    [Space(10)]

    public List<LearnableMove> movesByLevel;
    public List<TM> movesByMachine;

    [Header("Evolution Info")]
    public Pokemon evolution;
    public int evolveLevel;
    public List<ItemEvolve> evolutionsByItem;

    [Space(10)]

    public Stats baseStats;
    public ExperienceRate experienceRate;

    [Header("Extra Info")]

    public int experienceYield;
    public int catchRate;
    public int walkDistance;

    [Space(10)]

    public AudioClip cry;
    public PokedexProfile profile;
}
[System.Serializable]
public class PokedexProfile
{
    public string pokedexSpecies;
    public string pokedexDescription;
    public string height;
    public string weight;
    public List<string> eggGroups;
    public string genderRatio;
    public PokemonModel models;

    public Sprite pokedexSprite;
    public Sprite gameSprite;
    public Sprite HDSprite;
}

[System.Serializable]
public class LearnableMove
{
    [HideInInspector] public string name;
    public Move move;
};
[System.Serializable]
public class PokemonModel
{
    public GameObject myMaleModel;
    public GameObject myFemaleModel;
    public GameObject myShinyMaleModel;
    public GameObject myShinyFemaleModel;
    public Sprite sprite;
    public Sprite shinySprite;
};
public enum Nature { Hardy, Lonely, Brave, Adamant, Naughty, Bold, Docile, Relaxed, Impish, Lax, Timid, Hasty, Serious, Jolly, Naive, Modest, Mild, Quiet, Bashful, Rash, Calm, Gentile, Sassy, Careful, Quirky }
public enum ExperienceRate { Erratic, Fast, MediumFast, MediumSlow, Slow, Fluctuating }