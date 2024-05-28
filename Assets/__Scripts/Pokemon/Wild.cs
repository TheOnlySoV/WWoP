using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[System.Serializable]
public class Wild
{
    public Pokemon pokemon;
    [Space(20)]
    public int level = 5;
    [Space(10)]
    public Gender gender;
    public Ability ability;
    public Nature nature;
    [Space(10)]
    public Stats IV;
    public Stats stats;//conditional
    [Space(10)]
    public ConditionStatus status = ConditionStatus.Healthy;//default
    public int currentHealth;//conditional
    [Space(10)]
    public List<LearnedMove> moveset = new List<LearnedMove>();//conditional
    [Space(10)]
    public GameObject myModel;
    [Space(10)]
    public bool shiny = false;
    [Space(20)]
    public bool inBattle = false;
    [Space(10)]
    public GameObject ball;

    public Wild(Pokemon pokemon, int level, Gender gender, Ability ability, Nature nature, Stats IV, GameObject myModel, bool shiny)
    {
        this.pokemon = pokemon;
        this.level = level;
        this.gender = gender;
        this.ability = ability;
        this.nature = nature;
        this.IV = IV;
        this.myModel = myModel;
        this.shiny = shiny;
    }

    public Wild()
    {

    }

    public void GetMoves()
    {
        for (int i = level - 1; i >= 0; i--)
        {
            if (pokemon.info.movesByLevel[i].move != null && moveset.Count < 4)
                moveset.Add(new LearnedMove(pokemon.info.movesByLevel[i].move, 0));
        }

        moveset.Reverse();
    }

    public void GetStats()
    {
        Stats EV = new Stats();
        stats = pokemon.CalculateStats(level, IV, EV);

        currentHealth = stats.HP;
    }

    public void Encounter()
    {

    }
}
