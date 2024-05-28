using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    [Space(10)]

    public List<GameObject> floorLootBall;

    [Space(10)]

    public RandomTier tier = RandomTier.Tier1;
    public BagSlot itemSlot;

    [Space(10)]

    public ItemLoot rolledLoot;

    [Header("Randomized Info")]
    public bool randomized = false;
    public LootTier lootTiers;
    [Range(30f, 600f)] 
    public float despawnTimer = 300f;
    [Range(1, 100)]
    public int spawnChance = 10;

    [Space(10)]

    [SerializeField]
    private FloorItemState state = FloorItemState.Respawning;//save this
    [SerializeField]
    private float respawnCountdown = 0f;//save this
    [SerializeField]
    private float despawnCountdown = 0f;//save this

    private void Awake()
    {
        respawnCountdown = despawnTimer;
        despawnCountdown = despawnTimer;
        if (state == FloorItemState.Obtained)
        {
            Destroy(gameObject);
            return;
        }

        if (!randomized)
        {
            if (transform.childCount < 2)
                Instantiate(floorLootBall[Random.Range(0, floorLootBall.Count)], transform.GetChild(0).position, Quaternion.Euler(transform.rotation.eulerAngles), transform.GetChild(0));
        }
        else
        {
            TryAgain();
        }
    }

    private void Update()
    {
        switch (state)
        {
            case FloorItemState.Obtained:
                Destroy(gameObject);
                return;
            case FloorItemState.Respawning:
                respawnCountdown -= Time.deltaTime;
                if (respawnCountdown <= 0f)
                    TryAgain();
                break;
            case FloorItemState.Spawned:
                despawnCountdown -= Time.deltaTime;
                if (despawnCountdown <= 0f)
                    TryAgain();
                    return;
        }
    }

    public bool SpawnCheck()
    {
        int rng = Random.Range(1, 101);
        if (rng < spawnChance)
            return true;
        return false;
    }

    public override void Interact(Transform player)
    {
        InventoryManager.instance.PickUp(this);
        Used();
    }

    public void Used()
    {
        ResetItem();
        if (randomized)
        {
            state = FloorItemState.Respawning;
            respawnCountdown = despawnTimer / 1.5f;
        }
        else
            state = FloorItemState.Obtained;
    }

    void TryAgain()
    {
        ResetItem();

        if (SpawnCheck())
            SpawnRandom();
        else
        {
            state = FloorItemState.Respawning;
            respawnCountdown = despawnTimer / 2f;
        }
    }

    public void ResetItem()
    {
        if (transform.GetChild(0).childCount > 1)
            Destroy(transform.GetChild(0).GetChild(1).gameObject);
        GetComponent<Collider>().enabled = false;
        transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Stop();
        rolledLoot = null;
    }

    void SpawnRandom()
    {
        switch (tier)
        {
            case RandomTier.Tier1:
                SpawnLootTier(lootTiers.tier1);
                break;

            case RandomTier.Tier2:
                SpawnLootTier(lootTiers.tier2);
                break;

            case RandomTier.Tier3:
                SpawnLootTier(lootTiers.tier3);
                break;
        }
    }

    void SpawnLootTier(List<RandomizedLoot> lootList)
    {
        int rng = Random.Range(0, lootList.Count);

        itemSlot = lootList[rng].itemSlot;
        rolledLoot = lootList[rng].loot;
        Instantiate(floorLootBall[Random.Range(0, floorLootBall.Count)], transform.GetChild(0).position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), transform.GetChild(0));
        GetComponent<Collider>().enabled = true;
        transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play();

        state = FloorItemState.Spawned;
        despawnCountdown = despawnTimer;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Player")
            Interact(other.transform);
    }
}
public enum FloorItemState { Obtained, Respawning, Spawned }
public enum BagSlot { Item, Healing, Ball, Move, Key }
public enum RandomTier { Tier1, Tier2, Tier3 }

[System.Serializable]
public class ItemLoot
{
    public List<HealingInventoryItem> healingItems;
    public List<TMInventoryItem> moveItems;
    public List<BagItem> items;
    public List<BallInventoryItem> ballLoot;
    public KeyInventoryItem keyItem;
};

[System.Serializable]
public class RandomizedLoot
{
    [HideInInspector]
    public string name;
    public BagSlot itemSlot;
    public ItemLoot loot;
};

[System.Serializable]
public class LootTier
{
    public List<RandomizedLoot> tier1;
    public List<RandomizedLoot> tier2;
    public List<RandomizedLoot> tier3;
};