using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Healing Item", menuName = "My Assets/New Heal")]
public class HealingItem : ScriptableObject //usable by default
{
    public Sprite sprite;

    public int healAmount;
    public bool revive = false;
    public bool maxRevive = false;

    [Header("Conditions")]
    public ConditionStatus healStatus;

    private void OnValidate()
    {
        if (maxRevive)
            revive = true;
    }

    public bool HealPokemon(int target)
    {
        InventoryManager im = InventoryManager.instance;
        switch (healStatus)
        {
            case ConditionStatus.Healthy:
                break;

            case ConditionStatus.Asleep:
                im.party[target].status = ConditionStatus.Healthy;
                break;

            case ConditionStatus.Burned:
                im.party[target].status = ConditionStatus.Healthy;
                break;

            case ConditionStatus.Frozen:
                im.party[target].status = ConditionStatus.Healthy;
                break;

            case ConditionStatus.Paralyzed:
                im.party[target].status = ConditionStatus.Healthy;
                break;

            case ConditionStatus.Poisoned:
                im.party[target].status = ConditionStatus.Healthy;
                break;
            case ConditionStatus.Fainted:
                im.party[target].status = ConditionStatus.Healthy;
                break;
        }
        if (healAmount > 0)
        {
            if (im.party[target].currentHealth + healAmount > im.party[target].stats.HP)
                im.party[target].currentHealth = im.party[target].stats.HP;
            else
                im.party[target].currentHealth += healAmount;
            return true;
        }
        if (revive)
        {
            im.party[target].currentHealth = im.party[target].stats.HP / 2;
            if (maxRevive)
                im.party[target].currentHealth = im.party[target].stats.HP;
            return true;
        }
        return false;
    }
}
