using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New item", menuName = "My Assets/New item")]
public class FloorItem : ScriptableObject
{
    public bool evolution = false;
    public FloorItemType itemType;
    public Sprite sprite;
    public bool EvolvePokemon(int index)
    {
        /*
        InventoryManager im = GameManager.instance.im;
        Critter cachedCritter = im.ownedCritters[index].critter;
        im.ownedCritters[index].critter = cachedCritter.evolutionsByItem[0].evolvesTo; 
        switch (im.ownedCritters[index].gender)
        {
            case Gender.None:
                if (im.ownedCritters[index].shiny)
                {
                    im.ownedCritters[index].myModel = im.ownedCritters[index].critter.myShinyMaleModel;
                    break;
                }
                else
                {
                    im.ownedCritters[index].myModel = im.ownedCritters[index].critter.myMaleModel;
                    break;
                }
            case Gender.Male:
                if (im.ownedCritters[index].shiny)
                {
                    im.ownedCritters[index].myModel = im.ownedCritters[index].critter.myShinyMaleModel;
                    break;
                }
                else
                {
                    im.ownedCritters[index].myModel = im.ownedCritters[index].critter.myMaleModel;
                    break;
                }
            case Gender.Female:
                if (im.ownedCritters[index].shiny)
                {
                    im.ownedCritters[index].myModel = im.ownedCritters[index].critter.myShinyFemaleModel;
                    break;
                }
                else
                {
                    im.ownedCritters[index].myModel = im.ownedCritters[index].critter.myFemaleModel;
                    break;
                }
            case Gender.Legendary:
                if (im.ownedCritters[index].shiny)
                {
                    im.ownedCritters[index].myModel = im.ownedCritters[index].critter.myShinyFemaleModel;
                    break;
                }
                else
                {
                    im.ownedCritters[index].myModel = im.ownedCritters[index].critter.myFemaleModel;
                    break;
                }
        }
        */
        return true;
    }

    public bool UseBattleItem(int index)
    {
        return false;
    }
}

public enum FloorItemType { BattleStatBooster, EVStatBooster, BattleUtility, Utility, Evolution }
