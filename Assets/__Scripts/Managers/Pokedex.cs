using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Pokedex : MonoBehaviour
{
    public static Pokedex instance;

    public int totalCaught = 0;

    //public TextAsset pokemonJSONFile;
    //public TextAsset moveJSONFile;
    //public TextAsset abilityJSONFile;
    //public TextAsset learnSetsJSONFile;

    public Transform contentParent;
    public GameObject pokedexEntryPrefab;

    public List<Region> pokedex;
    public List<Pokemon> addedPokemon;
    public List<Type> types;
    public List<Ability> abilities;
    public List<Move> moves;

    //public bool getJSONPokemonData = false;
    //public bool getJSONAbilityData = false;
    //public bool getJSONMoveData = false;
    //public bool getJSONLearnSetData = false;
    //public bool getJSONAudioData = false;

    private void Awake()
    {
        if (instance)
            Destroy(instance);

        instance = this;
    }

    private void Start()
    {
        //if (getJSONPokemonData)
        //    GetPokemon();
        //if (getJSONMoveData)
        //    GetMoves();
        //if (getJSONAbilityData)
        //    GetAbilities();
        //if (getJSONLearnSetData)
        //    GetLearnSets();
        //if (getJSONAudioData)
        //    AssignAudio();
        //int overallIndex = 0;
        //for(int i = 0; i < pokedex.Count; i++)
        //    for (int j = 0; j < pokedex[i].entries.Count; j++)
        //        pokedex[i].entries[j].pokemon = addedPokemon[overallIndex++];

        GetRegion(0);
    }

    public void GetRegion(int index)
    {
        ClearContent();

        DisplayRegion(pokedex[index].entries);
    }

    void ClearContent()
    {
        for(int i = 0; i < contentParent.childCount; i++)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }
    }

    void DisplayRegion(List<Entry> regionToDisplay)
    {
        for (int i = 0; i < regionToDisplay.Count; i++)
        {
            int cachedIndex = i;

            GameObject newDisplayedPokemon = Instantiate(pokedexEntryPrefab, contentParent);
            //newDisplayedPokemon.transform.GetChild(0).GetComponent<Image>().sprite = regionToDisplay[cachedIndex].pokemon.info.profile.pokedexSprite;
            newDisplayedPokemon.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{regionToDisplay[cachedIndex].pokemon.info.pID}";
            newDisplayedPokemon.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = regionToDisplay[cachedIndex].pokemon.info.pokemonName;
            //newDisplayedPokemon.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = string.Empty;
            newDisplayedPokemon.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{regionToDisplay[cachedIndex].totalObtained}";

            //Button button = newDisplayedPokemon.GetComponent<Button>();
        }
    }

    #region Hiding
    //public void GetPokemon()
    //{
    //    int index = 1;
    //    PokemonData data = JsonUtility.FromJson<PokemonData>(pokemonJSONFile.text);
    //    foreach(JSONPokemonData pd in data.pokemon)
    //    {
    //        bool added = false;
    //        foreach(Pokemon pokemon in addedPokemon)
    //        {
    //            if (pd.name.english == pokemon.info.pokemonName)
    //            {
    //                added = true;
    //                pokemon.info.pID = pd.id;
    //                pokemon.info.pokemonName = pd.name.english;
    //                pokemon.info.type1 = AssignType(pd.type[0]);

    //                if (pd.type.Count > 1)
    //                    pokemon.info.type2 = AssignType(pd.type[1]);
    //                else
    //                    pokemon.info.type2 = AssignType("None");

    //                pokemon.info.baseStats.HP = pd.baseStats.HP;
    //                pokemon.info.baseStats.attack = pd.baseStats.Attack;
    //                pokemon.info.baseStats.defense = pd.baseStats.Defense;
    //                pokemon.info.baseStats.specialAttack = pd.baseStats.specialAttack;
    //                pokemon.info.baseStats.specialDefense = pd.baseStats.specialDefense;
    //                pokemon.info.baseStats.speed = pd.baseStats.Speed;

    //                pokemon.info.profile.pokedexSpecies = pd.species;
    //                pokemon.info.profile.pokedexDescription = pd.description;
    //                pokemon.info.profile.height = pd.profile.height;
    //                pokemon.info.profile.weight = pd.profile.weight;
    //                pokemon.info.profile.eggGroups.Clear();
    //                if (pd.profile.egg.Count > 0)
    //                {
    //                    pokemon.info.profile.eggGroups.Add(pd.profile.egg[0]);
    //                    if (pd.profile.egg.Count > 1)
    //                        pokemon.info.profile.eggGroups.Add(pd.profile.egg[1]);
    //                }

    //                pokemon.info.profile.genderRatio = pd.profile.gender;

    //                index++;

    //                AssetDatabase.Refresh();
    //                EditorUtility.SetDirty(pokemon);
    //                AssetDatabase.SaveAssets();
    //            }
    //        }
    //        if (!added)
    //        {
    //            print($"{pd.name.english} is not in database. Adding {pd.name.english}.");
    //            Pokemon newPokemon = ScriptableObject.CreateInstance<Pokemon>();
    //            newPokemon.info = new PokemonInfo();

    //            newPokemon.info.pID = pd.id;
    //            newPokemon.info.pokemonName = pd.name.english;
    //            newPokemon.info.type1 = AssignType(pd.type[0]);

    //            if (pd.type.Count > 1)
    //                newPokemon.info.type2 = AssignType(pd.type[1]);
    //            else
    //                newPokemon.info.type2 = AssignType("None");

    //            newPokemon.info.baseStats = new Stats();

    //            newPokemon.info.baseStats.HP = pd.baseStats.HP;
    //            newPokemon.info.baseStats.attack = pd.baseStats.Attack;
    //            newPokemon.info.baseStats.defense = pd.baseStats.Defense;
    //            newPokemon.info.baseStats.specialAttack = pd.baseStats.specialAttack;
    //            newPokemon.info.baseStats.specialDefense = pd.baseStats.specialDefense;
    //            newPokemon.info.baseStats.speed = pd.baseStats.Speed;

    //            newPokemon.info.profile = new PokedexProfile();

    //            newPokemon.info.profile.pokedexSpecies = pd.species;
    //            newPokemon.info.profile.pokedexDescription = pd.description;
    //            newPokemon.info.profile.height = pd.profile.height;
    //            newPokemon.info.profile.weight = pd.profile.weight;
    //            if (pd.profile.egg.Count > 0)
    //            {
    //                newPokemon.info.profile.eggGroups = new List<string>() { pd.profile.egg[0] };
    //                if (pd.profile.egg.Count > 1)
    //                    newPokemon.info.profile.eggGroups.Add(pd.profile.egg[1]);
    //            }

    //            newPokemon.info.profile.genderRatio = pd.profile.gender;

    //            if (pd.id == 772)
    //                AssetDatabase.CreateAsset(newPokemon, $"Assets/__Scripts/Scriptables/Pokemon/{pd.id} - Type Null.asset");
    //            else
    //                AssetDatabase.CreateAsset(newPokemon, $"Assets/__Scripts/Scriptables/Pokemon/{pd.id} - {pd.name.english}.asset");

    //            AssetDatabase.Refresh();
    //            EditorUtility.SetDirty(newPokemon);
    //            AssetDatabase.SaveAssets();

    //            addedPokemon.Add(newPokemon);
    //        }
    //    }

    //    AssetDatabase.Refresh();
    //    AssetDatabase.SaveAssets();
    //}

    //public void GetMoves()
    //{
    //    MoveData data = JsonUtility.FromJson<MoveData>(moveJSONFile.text);
    //    foreach (JSONMoveData md in data.move)
    //    {
    //        bool added = false;
    //        foreach (Move moveToUpdate in moves)
    //        {
    //            if (md.name == moveToUpdate.name)
    //            {
    //                added = true;
    //                moveToUpdate.powerPoints = md.pp;
    //                moveToUpdate.description = md.description;
    //                moveToUpdate.basePower = md.power;
    //                moveToUpdate.accuracy = md.accuracy;

    //                switch (md.category)
    //                {
    //                    case "Special":
    //                        moveToUpdate.category = Category.Special;
    //                        break;
    //                    case "Physical":
    //                        moveToUpdate.category = Category.Physical;
    //                        break;
    //                    case "Non - Damaging":
    //                        moveToUpdate.category = Category.Other;
    //                        break;
    //                }

    //                moveToUpdate.battleType = AssignType(md.type);

    //                AssetDatabase.Refresh();
    //                EditorUtility.SetDirty(moveToUpdate);
    //                AssetDatabase.SaveAssets();
    //            }
    //        }
    //        if (!added)
    //        {
    //            print($"{md.name} is not in database. Adding {md.name}.");
    //            Move newMove = ScriptableObject.CreateInstance<Move>();

    //            newMove.name = md.name;
    //            newMove.powerPoints = md.pp;
    //            newMove.description = md.description;
    //            newMove.basePower = md.power;
    //            newMove.accuracy = md.accuracy;

    //            switch (md.category)
    //            {
    //                case "Special":
    //                    newMove.category = Category.Special;
    //                    break;
    //                case "Physical":
    //                    newMove.category = Category.Physical;
    //                    break;
    //                case "Non - Damaging":
    //                    newMove.category = Category.Other;
    //                    break;
    //            }

    //            newMove.battleType = AssignType(md.type);

    //            AssetDatabase.CreateAsset(newMove, $"Assets/__Scripts/Scriptables/Moves/{md.name}.asset");

    //            AssetDatabase.Refresh();
    //            EditorUtility.SetDirty(newMove);
    //            AssetDatabase.SaveAssets();
    //        }
    //    }

    //    AssetDatabase.Refresh();
    //    AssetDatabase.SaveAssets();
    //}

    //public void GetAbilities()
    //{
    //    AbilityData data = JsonUtility.FromJson<AbilityData>(abilityJSONFile.text);
    //    foreach(JSONAbilityData ad in data.ability)
    //    {
    //        bool added = false;
    //        foreach (Ability ability in abilities)
    //        {
    //            if (ad.name == ability.name)
    //            {
    //                ability.description = ad.description;
    //                added = true;
    //                AssetDatabase.Refresh();
    //                EditorUtility.SetDirty(ability);
    //                AssetDatabase.SaveAssets();
    //            }
    //        }
    //        if (!added)
    //        {
    //            print($"{ad.name} is not in database. Adding {ad.name}.");
    //            Ability newAbility = ScriptableObject.CreateInstance<Ability>();

    //            newAbility.name = ad.name;
    //            newAbility.description = ad.description;

    //            AssetDatabase.CreateAsset(newAbility, $"Assets/__Scripts/Scriptables/Abilities/{ad.name}.asset");

    //            AssetDatabase.Refresh();
    //            EditorUtility.SetDirty(newAbility);
    //            AssetDatabase.SaveAssets();
    //        }

    //        AssetDatabase.Refresh();
    //        AssetDatabase.SaveAssets();
    //    }
    //}

    //public void GetLearnSets()
    //{
    //    string path = @"C:\Users\south\OneDrive\Desktop\Unity Projects (Move off of SSD)\The WWoP\Assets\__Scripts\Managers\TextFiles\New Learn Sets File ALT.txt";
    //    List<string> lines = File.ReadAllLines(path).ToList();
    //    for (int i = 0; i < lines.Count; i++)
    //    {
    //        int cachedIndex = i;
    //        foreach (Pokemon pkmn in addedPokemon)
    //        {
    //            string cachedName = pkmn.info.pokemonName.ToLower();
    //            if (cachedName == "natu" || cachedName == "geodude" || cachedName == "crobat" || cachedName == "mew" || cachedName == "pidgeot"
    //                || cachedName == "rattata" || cachedName == "raticate" || cachedName == "raichu" || cachedName == "sandshrew" || cachedName == "sandslash"
    //                || cachedName == "vulpix" || cachedName == "hypno" || cachedName == "ninetails" || cachedName == "paras" || cachedName == "diglett"
    //                || cachedName == "dugtrio" || cachedName == "meowth" || cachedName == "persian" || cachedName == "abra" || cachedName == "graveler" || cachedName == "golem"
    //                || cachedName == "grimer" || cachedName == "muk" || cachedName == "exeggutor" || cachedName == "marowak" || cachedName == "porygon" || cachedName == "kabuto"
    //                || cachedName == "marill" || cachedName == "aron" || cachedName == "wormadam" || cachedName == "rotom" || cachedName == "woobat" || cachedName == "klink"
    //                || cachedName == "klang" || cachedName == "kyurem" || cachedName == "greninja" || cachedName == "meowstic" || cachedName == "lycanroc")
    //            { continue; }
    //            if (lines[cachedIndex].Contains(cachedName))
    //            {
    //                //pkmn.info.movesByLevel;
    //                //pkmn.info.movesByMachine;
    //                //print(pkmn.info.pokemonName.ToLower());
    //                //print(lines[cachedIndex]);
    //                string movesByLevelLine = lines[cachedIndex + 1].Remove(0, 9);//trim front
    //                print(cachedName);
    //                movesByLevelLine = movesByLevelLine.Remove(movesByLevelLine.Length - 2, 2);//trim back
    //                //every even property is a usable value
    //                //0 needs to be read
    //                //starting at 2, every 4th property is the level
    //                //starting at 4, every 4th property is the move name

    //                string[] readableLine = movesByLevelLine.Split(',');
    //                for (int j = 0; j < readableLine.Length; j++)
    //                {
    //                    string[] learnSet = readableLine[j].Split(':');
    //                    learnSet[0] = learnSet[0].Replace("\"", "");
    //                    learnSet[1] = learnSet[1].Replace("\"", "");

    //                    int level = int.Parse(learnSet[0]);
    //                    string moveName = learnSet[1];

    //                    //print($"{pkmn.info.pokemonName} can learn {FindMoveByName(moveName)} at level {level}");

    //                    if (FindMoveByName(moveName) != null)
    //                        pkmn.info.movesByLevel[level - 1].move = FindMoveByName(moveName);

    //                }
    //                AssetDatabase.Refresh();
    //                EditorUtility.SetDirty(pkmn);
    //                AssetDatabase.SaveAssets();
    //                string movesByMachineLine = lines[cachedIndex + 2].Remove(0, 4);
    //                {

    //                }
    //            }
    //        }
    //    }
    //}

    //public Move FindMoveByName(string arg)
    //{
    //    foreach (Move move in moves)
    //    {
    //        string moveToCheckAgainst = move.name.ToLower();
    //        moveToCheckAgainst = moveToCheckAgainst.Replace(" ", "");
    //        if (moveToCheckAgainst == arg)
    //            return move;
    //    }

    //    return null;
    //}
    #endregion
    public void AssignAudio()
    {
        foreach (Region region in pokedex)
        {
            foreach (Entry entry in region.entries)
            {
                if (region.regionName == "Galar")
                    return;
                string pID = entry.pokemon.info.pID.ToString().PadLeft(3, '0');
                string searchStr = $"{pID} - {region.regionName} - {entry.pokemon.info.pokemonName}";
                //print(searchStr);
                AudioClip cryToAssign = (AudioClip)AssetDatabase.LoadAssetAtPath($"Assets/Audio/Pokemon/Cries/{searchStr}.wav", typeof(AudioClip));

                entry.pokemon.info.cry = cryToAssign;

                AssetDatabase.Refresh();
                EditorUtility.SetDirty(entry.pokemon);
                AssetDatabase.SaveAssets();
            }
        }
    }

    public Type AssignType(string typeToAssign)
    {
        switch (typeToAssign)
        {
            case "None":
                return types[0];
            case "Bug":
                return types[1];
            case "Dark":
                return types[2];
            case "Dragon":
                return types[3];
            case "Electric":
                return types[4];
            case "Fairy":
                return types[5];
            case "Fighting":
                return types[6];
            case "Fire":
                return types[7];
            case "Flying":
                return types[8];
            case "Ghost":
                return types[9];
            case "Grass":
                return types[10];
            case "Ground":
                return types[11];
            case "Ice":
                return types[12];
            case "Normal":
                return types[13];
            case "Poison":
                return types[14];
            case "Psychic":
                return types[15];
            case "Rock":
                return types[16];
            case "Steel":
                return types[17];
            case "Water":
                return types[18];
            default: 
                return types[0];
        }
    }
}
#region Data Classes
[Serializable]
public class PokemonData
{
    public JSONPokemonData[] pokemon;
}

[Serializable]
public class JSONPokemonData
{
    public int id;
    public PokemonName name;
    public List<string> type;
    public PokemonStats baseStats;
    public string species;
    public string description;
    public List<EvolutionData> evolution;
    public Profile profile;
}

[Serializable]
public class EvolutionData
{
    public List<string> next;
}

[Serializable]
public class Profile
{
    public string height;
    public string weight;
    public List<string> egg;
    public string gender;
}

[Serializable]
public class GenderData
{
    public float maleChance;
    public float femaleChance;
}

[Serializable]
public class PokemonName
{
    public string english;
    public string japanese;
    public string chinese;
    public string french;
}

[Serializable]
public class PokemonStats
{
    public int HP;
    public int Attack;
    public int Defense;
    public int specialAttack;
    public int specialDefense;
    public int Speed;
}

[Serializable]
public class Region
{
    public string regionName;
    public List<Entry> entries;
}

[Serializable]
public class JSONMoveData
{
    public string name;
    public string category;
    public int power;
    public int accuracy;
    public int pp;
    public string description;
    public string type;
}

[Serializable]
public class MoveData
{
    public JSONMoveData[] move;
}

[Serializable]
public class AbilityData
{
    public JSONAbilityData[] ability;
}

[Serializable]
public class JSONAbilityData
{
    public string name;
    public string description;
}
#endregion

[Serializable]
public class Entry
{
    public Pokemon pokemon;

    public int totalObtained;
    public bool obtained;
    public bool obtainedShiny;
}