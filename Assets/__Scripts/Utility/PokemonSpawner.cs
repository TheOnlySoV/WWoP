using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PokemonSpawner : Spawner
{
    public GameObject wildPokemonPrefab;
    public SpawnState state;
    public List<Pokemon> spawns;
    public Pokemon rareSpawn;

    public int maxFloorSpawns;

    [Range(0.1f, 5f)]
    public float spawnDelay;
    float countdown = 0f;


    [Range(2, 90)]
    public int minLevel;
    [Range(4, 100)]
    public int maxLevel;
    [Range(1f, 300f)]
    public float despawnTimer = 30f;

    public bool getSpawnPoints = false;
    public List<SpawnPoint> wildSpawns;

    public List<Transform> trainersInSpawnArea;

    private void Start()
    {
        wildSpawns = new List<SpawnPoint>();
        //state = SpawnState.Idle;//Get current state from server
        countdown = spawnDelay;//get current countdown value from server
    }
    private void Update()
    {
        switch(state)
        {
            case SpawnState.Idle:
                if (wildSpawns.Count < maxFloorSpawns)
                    state = SpawnState.Spawning;
                return;
            case SpawnState.Full:
                countdown = spawnDelay;
                return;
            case SpawnState.Spawning:
                countdown -= Time.deltaTime;
                if (countdown <= 0f)
                    SpawnPokemon();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trainer") && !trainersInSpawnArea.Contains(other.transform))
            trainersInSpawnArea.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Trainer") && trainersInSpawnArea.Contains(other.transform))
            trainersInSpawnArea.Remove(other.transform);
    }

    public void SpawnPokemon()
    {
        List<Pokemon> pokemonSpawns = new List<Pokemon>(spawns);
        if (rareSpawn)
            pokemonSpawns.Add(rareSpawn);

        SpawnPoint nextSpawnPoint = new SpawnPoint(transform.GetChild(Random.Range(0, transform.childCount)), true);

        GameObject newParent = Instantiate(wildPokemonPrefab, SpawnLocation(nextSpawnPoint), Quaternion.Euler(transform.eulerAngles));

        Pokemon RNG = pokemonSpawns[Random.Range(0, pokemonSpawns.Count)];
        Gender gender = RNG.AssignGender();
        bool shiny = RNG.ShinyCheck();
        Nature nature = RNG.AssignNature();
        GameObject myModel = RNG.GetModel(shiny, gender);
        Stats IV = RNG.RollIV();
        int level = Random.Range(minLevel, maxLevel + 1);
        Ability ability = RNG.info.hiddenAbility;

        Wild newWildPokemon = new Wild(RNG, level, gender, ability, nature, IV, myModel, shiny);
        newWildPokemon.GetMoves();
        newWildPokemon.GetStats();
        newParent.GetComponent<PokemonMotor>().pokemon = RNG;
        newParent.GetComponent<PokemonMotor>().Spawner = this;
        newParent.GetComponent<PokemonMotor>().mySpawnerIndex = wildSpawns.Count;
        newParent.GetComponent<PokemonMotor>().wild = newWildPokemon;
        newParent.GetComponent<PokemonMotor>().despawnTimer = despawnTimer;

        nextSpawnPoint.spawn = newWildPokemon;
        nextSpawnPoint.hook = newParent.transform;
        wildSpawns.Add(nextSpawnPoint);

        Instantiate(myModel, newParent.transform);
        newParent.GetComponent<Animator>().runtimeAnimatorController = RNG.controller;
        newParent.GetComponent<Animator>().avatar = RNG.avatar;

        float fRNG = Random.Range(0, 360);
        newParent.transform.eulerAngles = new Vector3(0f, fRNG, 0f);

        //newParent.GetComponent<PokemonMotor>().GoToNewLocation(new Vector3(Random.Range(-spawnRange.x, spawnRange.x), 0f, Random.Range(-spawnRange.z, spawnRange.z)));//replace with raycast, and have server trigger it

        if (wildSpawns.Count >= maxFloorSpawns)
        {
            state = SpawnState.Full;
        }
        else
            countdown = spawnDelay;
    }

    Vector3 SpawnLocation(SpawnPoint spawnPoint)
    {
        spawnPoint.active = true;
        Ray ray = new Ray(spawnPoint.point.position, -Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayer))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    public void RestorePoint(int index)
    {
        wildSpawns.RemoveAt(index);

        for (int i = 0; i < wildSpawns.Count; i++)
        {
            wildSpawns[i].hook.GetComponent<PokemonMotor>().mySpawnerIndex = i;
        }

        if (state == SpawnState.Full)
            state = SpawnState.Idle;
    }

    public Vector3 NewMovementLocation()
    {
        Vector3 returnLocation = Vector3.zero;

        Ray ray = new Ray(new Vector3(Random.Range(-spawnRange.x, spawnRange.x) + transform.position.x, transform.position.y, Random.Range(-spawnRange.z, spawnRange.z) + transform.position.z), -Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayer))
        {
            returnLocation = hit.point;
        }

        return returnLocation;
    }
}

[System.Serializable]
public class SpawnPoint
{
    public Transform point;
    public Wild spawn;
    public bool active;

    [HideInInspector]
    public Transform hook; 

    public SpawnPoint(Transform point, bool active)
    {
        this.point = point;
        this.active = active;
    }
}