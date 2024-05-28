using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PCManager : MonoBehaviour
{
    public SelectionMode selectionMode;

    InventoryManager im;

    public static PCManager instance;

    public int currentBox = -1;

    public Transform storedParent;
    public Transform partyParent;

    public TextMeshProUGUI boxHeader;

    public SelectedPCPokemon selectedBoxPokemon;
    public SelectedPCPokemon selectedPartyPokemon;

    private void Awake()
    {
        if (instance)
            Destroy(instance);

        instance = this;
    }

    private void Start()
    {
        im = InventoryManager.instance;

        selectedBoxPokemon = new SelectedPCPokemon(new Vector2Int(-1, -1), null);
        selectedPartyPokemon = new SelectedPCPokemon(new Vector2Int(0, -1), null);
    }

    public void OpenPC()
    {
        currentBox = 0;

        ClearSelections();

        GameManager.instance.trainer.GetComponent<PlayerControllerInputs>().playerInputState = PlayerState.Paused;

        GetBox();
        GetParty();
    }

    public void ClosePC()
    {
        currentBox = -1;

        ClearSelections();

        GameManager.instance.trainer.GetComponent<PlayerControllerInputs>().playerInputState = PlayerState.Playing;
    }

    public void MoveToNext()
    {
        currentBox++;
        if (currentBox > im.stored.Count - 1)
            currentBox = 0;

        GetBox();
    }

    public void MoveToPrevious()
    {
        currentBox--;
        if (currentBox < 0)
            currentBox = im.stored.Count - 1;

        GetBox();
    }

    public void GetBox()
    {
        for (int i = 0; i < im.stored[currentBox].box.Count; i++)
        {
            if (im.stored[currentBox].box[i].pokemon != null)
            {
                //storedParent.GetChild(i).GetChild(0).GetComponent<Image>().sprite = im.stored[currentBox].box[i].pokemon.info.profile.gameSprite;
                storedParent.GetChild(i).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Lv {im.stored[currentBox].box[i].level}";
                storedParent.GetChild(i).GetChild(0).gameObject.SetActive(true);
            } else
            {
                storedParent.GetChild(i).GetChild(0).GetComponent<Image>().sprite = null;
                storedParent.GetChild(i).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Lv 0";
                storedParent.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }

        //change header
        boxHeader.text = $"Box {currentBox + 1}";
    }

    public void GetParty()
    {
        for (int i = 0; i < 6; i++)
        {
            if (i < im.party.Count)
            {
                //display the slot
                //partyParent.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = im.party[i].pokemon.info.profile.gameSprite;
                partyParent.GetChild(i).GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = im.party[i].status.ToString();
                partyParent.GetChild(i).GetChild(0).GetChild(1).gameObject.SetActive(true);
                partyParent.GetChild(i).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = im.party[i].pokemon.info.pokemonName;
                partyParent.GetChild(i).GetChild(0).GetChild(2).gameObject.SetActive(true);
                partyParent.GetChild(i).GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = $"Level {im.party[i].level}";
                partyParent.GetChild(i).GetChild(0).GetChild(3).gameObject.SetActive(true);
                partyParent.GetChild(i).GetChild(0).GetChild(4).GetComponent<Slider>().maxValue = im.party[i].stats.HP;
                partyParent.GetChild(i).GetChild(0).GetChild(4).GetComponent<Slider>().value = im.party[i].currentHealth;
                partyParent.GetChild(i).GetChild(0).GetChild(4).gameObject.SetActive(true);

                //partyParent.GetChild(i).GetChild(0).GetComponent<Button>().interactable = true;
            }
            else
            {
                //clear the party slot
                partyParent.GetChild(i).GetChild(0).GetChild(1).gameObject.SetActive(false);
                partyParent.GetChild(i).GetChild(0).GetChild(2).gameObject.SetActive(false);
                partyParent.GetChild(i).GetChild(0).GetChild(3).gameObject.SetActive(false);
                partyParent.GetChild(i).GetChild(0).GetChild(4).gameObject.SetActive(false);

                //partyParent.GetChild(i).GetChild(0).GetComponent<Button>().interactable = false;
            }
        }

        UIManager.instance.UpdatePlayerUIParty();
    }

    public (int box, int slot) AddToBox(Owned target)
    {
        (int b, int s) returnableBoxSlot = new (-1, -1);
        for (int i = 0; i < im.stored.Count; i++)
        {
            for (int j = 0; j < im.stored[i].box.Count; j++)
            {
                //found an empty box slot
                if (im.stored[i].box[j].pokemon == null)
                {
                    returnableBoxSlot.b = i;
                    returnableBoxSlot.s = j;

                    im.stored[i].box[j] = target;
                    return returnableBoxSlot;
                }
            }
        }
        if (returnableBoxSlot.b < 0 || returnableBoxSlot.s < 0)
        {
            //make a new box
            List<Owned> newBox = new List<Owned>(im.maxPokemonPerBox);
            newBox[0] = target;

            returnableBoxSlot.b = im.stored.Count;
            returnableBoxSlot.s = im.stored[returnableBoxSlot.b].box.Count;
            return returnableBoxSlot;
        }

        return returnableBoxSlot;
    }

    public void SetSelectionMode(int mode)
    {
        selectionMode = (SelectionMode)mode;
    }

    public void MovePCtoPCPokemon(SelectedPCPokemon secondSelected)
    {
        Owned cachedSelected = im.stored[selectedBoxPokemon.boxIndex.x].box[selectedBoxPokemon.boxIndex.y];

        im.stored[selectedBoxPokemon.boxIndex.x].box[selectedBoxPokemon.boxIndex.y] = im.stored[secondSelected.boxIndex.x].box[secondSelected.boxIndex.y];
        im.stored[secondSelected.boxIndex.x].box[secondSelected.boxIndex.y] = cachedSelected;

        ClearSelections();

        GetBox();
    }

    public void MovePCtoPartyPokemon(SelectedPCPokemon secondSelected)
    {
        Owned cachedSelected = im.party[selectedPartyPokemon.boxIndex.y];

        if (im.party.Count > 1)
            im.party.RemoveAt(selectedPartyPokemon.boxIndex.y);
        else
        {
            print("Cannot remove the last Pokemon in the party.");
            ClearSelections();
            return;
        }

        if (im.stored[secondSelected.boxIndex.x].box[secondSelected.boxIndex.y].pokemon)
            im.party[selectedPartyPokemon.boxIndex.y] = im.stored[secondSelected.boxIndex.x].box[secondSelected.boxIndex.y];

        im.stored[secondSelected.boxIndex.x].box[secondSelected.boxIndex.y] = cachedSelected;

        ClearSelections();

        GetBox();
        //refresh party
        GetParty();
    }

    public void GetIndex(int index)
    {
        if (selectedBoxPokemon.boxIndex.x > -1)
        {
            //swap the pokemon
            MovePCtoPCPokemon(new SelectedPCPokemon(new Vector2Int(currentBox, index), im.stored[currentBox].box[index]));
            return;
        }

        if (im.stored[currentBox].box[index].pokemon != null)
            SetSelectedBox(index);

        if (selectedPartyPokemon.boxIndex.x == -1)
        {
            selectedBoxPokemon = new SelectedPCPokemon(new Vector2Int(currentBox, index), null);
            MovePCtoPartyPokemon(selectedBoxPokemon);
        }
    }

    public void SetSelectedBox(int index)
    {
        Owned target = im.stored[currentBox].box[index];

        selectedBoxPokemon = new SelectedPCPokemon(new Vector2Int(currentBox, index), target);
    }

    public void SetSelectedParty(int index)
    {
        if (index < im.party.Count)
        {
            Owned target = im.party[index];

            selectedPartyPokemon = new SelectedPCPokemon(new Vector2Int(-1, index), target);
        }

        if (selectedBoxPokemon.boxIndex.x != -2)
        {
            im.party.Add(selectedBoxPokemon.selected);

            im.stored[selectedBoxPokemon.boxIndex.x].box[selectedBoxPokemon.boxIndex.y] = new Owned();

            ClearSelections();
            GetBox();
            GetParty();
        }
    }

    void ClearSelections()
    {
        selectedBoxPokemon = new SelectedPCPokemon(new Vector2Int(-2, -1), null);
        selectedPartyPokemon = new SelectedPCPokemon(new Vector2Int(-2, -1), null);
    }
}

[Serializable]
public class SelectedPCPokemon
{
    public Vector2Int boxIndex;//x = box, y = index - if x = -1, the target is in the party
    public Owned selected;

    public SelectedPCPokemon(Vector2Int boxIndex, Owned selected)
    {
        this.boxIndex = boxIndex;
        this.selected = selected;
    }
}

public enum SelectionMode { Single, Instant, Group }