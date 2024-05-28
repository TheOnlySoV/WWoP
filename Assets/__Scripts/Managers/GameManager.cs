using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject trainer;

    public GameObject followerPrefab;

    public System.Random rngSeed1;
    public System.Random rngSeed2;

    public CharacterState ballHook = CharacterState.OnLand;

    public static GameManager instance;
    InventoryManager im;

    public bool exitToMenu = true;
    private void Awake()
    {
        if (instance)
            Destroy(instance);
        instance = this;
        
        im = InventoryManager.instance;

        rngSeed1 = new System.Random();
        rngSeed2 = new System.Random();
    }

    public void SpawnFollower(int index, Vector3 location)
    {
        Vector3 pokemonDirection = (trainer.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(pokemonDirection.x, 0, pokemonDirection.z));

        GameObject newParent = Instantiate(followerPrefab, location, lookRotation);
        GameObject newFollower = Instantiate(im.party[index].myModel, newParent.transform);
        Pokemon cachedPokemon = im.party[index].pokemon;
        newParent.GetComponent<Animator>().runtimeAnimatorController = cachedPokemon.controller;
        newParent.GetComponent<Animator>().avatar = cachedPokemon.avatar;
    }

    public void CloseMenu()
    {
        PlayerControllerInputs input = trainer.GetComponent<PlayerControllerInputs>();
        switch (input.playerInputState)
        {
            case PlayerState.Paused:
                input.SetPlayerState((int)PlayerState.Playing);
                pauseMenu.SetActive(false);
                break;
            case PlayerState.Emote:
                input.SetPlayerState((int)PlayerState.Playing);
                pauseMenu.SetActive(false);
                break;

            default: break;
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
    }

    public void ExitButton()
    {
        if (exitToMenu)
            ExitToMenu();
        else
            ExitGame();
    }

    void ExitGame()
    {
        Application.Quit();
    }

    void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }
}

public enum CharacterState { OnLand, InCave, Surfing }