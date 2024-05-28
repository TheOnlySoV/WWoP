using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

[System.Serializable]
public class Owned
{
    public Pokemon pokemon;
    [Space(20)]
    public string nickname = string.Empty;//exclusive
    public string tID;//exclusive
    public int level = 5;
    [Space(10)]
    public Gender gender;
    public Ability ability;
    public Nature nature;
    [Space(10)]
    public Stats IV;
    public Stats stats;
    public Stats EV;//exclusive
    [Space(10)]
    public ConditionStatus status = ConditionStatus.Healthy;
    public int currentHealth;
    [Space(10)]
    public List<LearnedMove> moveset = new List<LearnedMove>();
    [Space(10)]
    public GameObject myModel;
    [Space(10)]
    public int nextLevelEXP;//exclusive
    public int EXPToNextLevel;//exclusive
    public int currentEXP;//exclusive
    [Space(10)]
    public bool shiny = false;
    [Space(10)]
    public GameObject ball;

    //hidden
    [HideInInspector]
    public bool leveledUp;

    public Owned() { }
}

[System.Serializable]
public class LearnedMove
{
    public Move move;
    public int timesUsed;

    public LearnedMove(Move move, int timesUsed)
    {
        this.move = move;
        this.timesUsed = timesUsed;
    }
}

