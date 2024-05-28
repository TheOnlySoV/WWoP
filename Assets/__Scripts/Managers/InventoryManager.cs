using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class InventoryManager : MonoBehaviour
{
    public List<Owned> party;
    [Range(0, 100)]
    public int maxPokemonPerBox = 30;
    public List<PCBox> stored;

    [SerializeField]
    private int cachedSelected = -1;

    public TrainerBag bag;



    [HideInInspector]
    public PlayerControllerInputs _input;
    [HideInInspector]
    public PlayerController _controller;

    #region Singleton
    public static InventoryManager instance;

    private void Awake()
    {
        if (instance)
            Destroy(instance);
        instance = this;
    }
    #endregion

    private void OnValidate()
    {
        for (int i = 0; i < stored.Count; ++i)
        {
            stored[i].name = $"Box {i + 1}";
            if (stored[i].box.Count > maxPokemonPerBox)
            {
                //reorganize
            }
        }
    }

    private void Start()
    {
        if (bag.pokeballs.Count > 0)
            bag.ballIndex = 0;
    }

    private void Update()
    {
        SlotChangeCheck();
    }

    void SlotChangeCheck()
    {
        if (_input == null)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerInputs>();
            return;
        }

        if (_input.scrollValue != 0 && !_controller.changeLock)
        {
            if (_input.scrollValue > 0)
            {
                bag.ballIndex++;
                if (bag.ballIndex >= bag.pokeballs.Count)
                    bag.ballIndex = 0;
            }
            else if (_input.scrollValue < 0)
            {
                bag.ballIndex--;
                if (bag.ballIndex < 0)
                    bag.ballIndex = bag.pokeballs.Count - 1;
            }
            _input.scrollValue = 0;
            UIManager.instance.UpdateSelectedBallUI();
        }
    }

    public void CatchPokemon(Owned target)
    {
        string catchMessage = string.Empty;
        if (party.Count < 6)
        {
            party.Add(target);
            UIManager.instance.UpdatePlayerUIParty();
            catchMessage = $"{target.pokemon.info.pokemonName} was caught!";
        }
        else
        {
            (int box, int slot) addedLocation = PCManager.instance.AddToBox(target);
            if (addedLocation.box < 0 || addedLocation.slot < 0)
                catchMessage = $"Error adding {target.pokemon.info.pokemonName}.\nRemoving {target.pokemon.info.pokemonName} from game.";
            else
                catchMessage = $"Added {target.pokemon.info.pokemonName} to Box {addedLocation.box + 1}, Slot {addedLocation.slot + 1}!";
        }
        UIManager.instance.AddNewNotification(new Notification("Pokedex", catchMessage));
    }

    public void PickUp(Item item)
    {
        switch(item.itemSlot)
        {
            case BagSlot.Item:
                PickUpItems(item);
                break;

            case BagSlot.Healing:
                PickUpHealing(item);
                break;

            case BagSlot.Ball:
                PickUpPokeballs(item);
                break;

            case BagSlot.Move:
                PickUpMoves(item);
                break;

            case BagSlot.Key:
                PickUpKeyItem(item);
                break;
        }
    }

    public void PickUpItems(Item item)
    {
        List<BagItem> toBeAdded = new List<BagItem>();
        for (int i = 0; i < item.rolledLoot.items.Count; i++)
        {
            bool added = false;
            for (int j = 0; j < bag.itemBag.Count; j++)
            {
                if (item.rolledLoot.items[i].item == bag.itemBag[j].item)
                {
                    bag.itemBag[j].quantity += item.rolledLoot.items[i].quantity;
                    added = true;
                }
            }

            if (!added)
                toBeAdded.Add(item.rolledLoot.items[i]);
        }
        foreach (BagItem bagItem in toBeAdded)
            bag.itemBag.Add(bagItem);
    }

    public void PickUpHealing(Item item)
    {
        List<HealingInventoryItem> toBeAdded = new List<HealingInventoryItem>();
        for (int i = 0; i < item.rolledLoot.healingItems.Count; i++)
        {
            bool added = false;
            for (int j = 0; j < bag.healingBag.Count; j++)
            {
                if (item.rolledLoot.healingItems[i].item == bag.healingBag[j].item)
                {
                    bag.healingBag[j].quantity += item.rolledLoot.healingItems[i].quantity;
                    added = true;
                }
            }

            if (!added)
                toBeAdded.Add(item.rolledLoot.healingItems[i]);
        }
        foreach (HealingInventoryItem healthItem in toBeAdded)
            bag.healingBag.Add(healthItem);
    }

    public void PickUpPokeballs(Item item)
    {
        List<BallInventoryItem> toBeAdded = new List<BallInventoryItem>();
        for (int i = 0; i < item.rolledLoot.ballLoot.Count; i++)
        {
            bool added = false;
            for (int j = 0; j < bag.pokeballs.Count; j++)
            {
                if (item.rolledLoot.ballLoot[i].ball == bag.pokeballs[j].ball)
                {
                    bag.pokeballs[j].quantity += item.rolledLoot.ballLoot[i].quantity;
                    added = true;
                }
            }

            if (!added)
                toBeAdded.Add(item.rolledLoot.ballLoot[i]);
        }
        foreach (BallInventoryItem ball in toBeAdded)
            bag.pokeballs.Add(ball);

        UIManager.instance.UpdateSelectedBallUI();
    }

    public void PickUpMoves(Item item)
    {
        List<TMInventoryItem> toBeAdded = new List<TMInventoryItem>();
        for (int i = 0; i < item.rolledLoot.moveItems.Count; i++)
        {
            bool added = false;
            for (int j = 0; j < bag.moveBag.Count; j++)
            {
                if (item.rolledLoot.moveItems[i].item.move == bag.moveBag[j].item.move)
                {
                    bag.moveBag[j].quantity += item.rolledLoot.moveItems[i].quantity;
                    added = true;
                }
            }

            if (!added)
                toBeAdded.Add(item.rolledLoot.moveItems[i]);
        }
        foreach (TMInventoryItem move in toBeAdded)
            bag.moveBag.Add(move);
    }

    public void PickUpKeyItem(Item item)
    {
        bag.keyItems.Add(item.rolledLoot.keyItem);
    }

    public void GetBadge(Item item)
    {

    }

    public Owned ConvertToOwned(Wild wild)
    {
        Owned returnedOwned = new Owned();

        returnedOwned.pokemon = wild.pokemon;
        returnedOwned.nickname = wild.pokemon.info.pokemonName;
        returnedOwned.tID = GameManager.instance.trainer.GetComponent<NetworkedTransform>().tID.ToString().PadLeft(6, '0');
        returnedOwned.level = wild.level;

        returnedOwned.gender = wild.gender;
        returnedOwned.ability = wild.ability;
        returnedOwned.nature = wild.nature;

        returnedOwned.IV = wild.IV;
        returnedOwned.stats = wild.stats;

        returnedOwned.EV = new Stats();

        returnedOwned.status = wild.status;
        returnedOwned.currentHealth = wild.currentHealth;

        returnedOwned.moveset = wild.moveset;
        returnedOwned.myModel = wild.myModel;

        returnedOwned.shiny = wild.shiny;

        returnedOwned.ball = wild.ball;

        return returnedOwned;
    }

    public void PartyButtonSelection(int index)
    {
        if (cachedSelected < 0)
        {
            cachedSelected = index;
            return;
        }

        Owned holder = party[cachedSelected];

        party[cachedSelected] = party[index];
        party[index] = holder;

        cachedSelected = -1;

        UIManager.instance.DisplayPokemon();
    }
}

[Serializable]
public class TrainerBag
{
    public int ballIndex = -1;

    [Space(10)]

    public List<HealingInventoryItem> healingBag = new List<HealingInventoryItem>(); //Healing Items
    public List<BallInventoryItem> pokeballs;
    public List<TMInventoryItem> moveBag = new List<TMInventoryItem>(); //Collected TMs
    public List<BagItem> itemBag = new List<BagItem>(); //All other items
    public List<KeyInventoryItem> keyItems = new List<KeyInventoryItem>(); //Key Items for the main story/side quests
    public List<GymBadge> gymBadges = new List<GymBadge>();
}

[Serializable]
public class FloorInventoryItem
{
    [HideInInspector] public string quickName;
    public FloorItem item;
    public int quantity;
}

[Serializable]
public class PCBox
{
    [HideInInspector]
    public string name;

    public List<Owned> box;
}

[Serializable]
public class HealingInventoryItem
{
    [HideInInspector]
    public string name;
    public HealingItem item;
    public int quantity = 1;
}

[Serializable]
public class TMInventoryItem
{
    [HideInInspector]
    public string name;
    public TM item;
    public int quantity = 1;
}

[Serializable]
public class BagItem
{
    [HideInInspector]
    public string name;
    public FloorItem item;
    public int quantity = 1;
}

[Serializable]
public class BallInventoryItem
{
    [HideInInspector] public string quickName;
    public Pokeball ball;
    public int quantity;
}

[Serializable]
public class KeyInventoryItem
{
    [HideInInspector]
    public string name;
    public Key item;
    public int quantity = 1;
}

[Serializable]
public class GymBadge
{
    [HideInInspector]
    public string name;
    public int slot;
    public bool obtained = false;
}