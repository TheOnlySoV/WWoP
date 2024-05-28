using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Ball", menuName = "My Assets/Pokeball")]
public class Pokeball : ScriptableObject
{
    [HideInInspector]
    public string name;
    public GameObject model;
    public GameObject throwable;
    [Range(.1f, 255f)] public float modifier;
    public BallOverride ballCalculation = BallOverride.Regular;
    public bool heal = false;
    public Sprite sprite;

    private void OnValidate()
    {
        name = string.Format($"{model.name} Ball");
    }

    public int ThrowPokeball(Wild target, Owned owned, bool trainerBattle = false)
    {
        if (trainerBattle)
            return -1;
        switch (ballCalculation)
        {
            case BallOverride.Regular:
                return CalculateCatch(target);

            case BallOverride.Masterball:
                return 4;

            case BallOverride.Dive:
                return CalculateDive(target);

            case BallOverride.Dream:
                return CalculateDream(target);

            case BallOverride.Dusk:
                return CalculateDusk(target);

            case BallOverride.Fast:
                return CalculateFast(target);

            case BallOverride.Heavy:
                return CalculateHeavy(target);

            case BallOverride.Level:
                return CalculateLevel(target, owned);

            case BallOverride.Love:
                return CalculateLove(target, owned);

            case BallOverride.Lure:
                return CalculateCatch(target);

            case BallOverride.Moon:
                return CalculateMoon(target);

            case BallOverride.Nest:
                return CalculateNest(target);

            case BallOverride.Net:
                return CalculateNet(target);

            case BallOverride.Quick:
                return CalculateQuick(target);

            case BallOverride.Timer:
                return CalculateTimer(target);

            case BallOverride.Repeat:
                return CalculateRepeat(target);

            default:
                return -1;
        }
    }

    int CalculateCatch(Wild target)
    {
        float a;//caputre evaluation
        float statusModifier = EvaluateStatusCondition(target);
        Stats stats = target.stats;
        a = (Mathf.Floor((((3 * stats.HP) - (2 * target.currentHealth)) * 4096 * target.pokemon.info.catchRate * modifier) / (3 * stats.HP))) * statusModifier;
        if (CalculateCritical(a))
        {
            return PerformShakeChecks(1, a);
        }
        return PerformShakeChecks(3, a);
    }

    int CalculateCatch(Wild target, float modOverride)
    {
        float a;//caputre evaluation
        float statusModifier = EvaluateStatusCondition(target);
        Stats stats = target.stats;
        a = (Mathf.Floor((((3 * stats.HP) - (2 * target.currentHealth)) * 4096 * target.pokemon.info.catchRate * modOverride) / (3 * stats.HP))) * statusModifier;
        if (CalculateCritical(a))
        {
            return PerformShakeChecks(1, a);
        }
        return PerformShakeChecks(3, a);
    }

    int PerformShakeChecks(int checks, float a)
    {
        float b;
        b = Mathf.Floor(65536 / Mathf.Sqrt(Mathf.Sqrt(1044480 / a)));
        for (int i = 1; i <= checks; i++)
        {
            int rng = Random.Range(0, 65536);
            if (b <= rng)
                return i;
        }
        return 4;
    }

    public bool PerformShakeCheck(float a)
    {
        float b;
        b = Mathf.Floor(65536 / Mathf.Sqrt(Mathf.Sqrt(1044480 / a)));
        int rng = Random.Range(0, 65536);

        if (b > rng)
            return true;

        return false;
    }

    public float EvaluateStatusCondition(Wild target)
    {
        switch(target.status)
        {
            case ConditionStatus.Healthy:
                return 1f;
            case ConditionStatus.Asleep:
                return 2.5f;
            case ConditionStatus.Frozen:
                return 2.5f;
            case ConditionStatus.Paralyzed:
                return 1.5f;
            case ConditionStatus.Poisoned:
                return 1.5f;
            case ConditionStatus.Burned:
                return 1.5f;
            default:
                return 1f;
        }
    }

    public bool CalculateCritical(float a)
    {
        float multiplier = 0f;
        //look at total pokemon obtained in the pokedex
        switch(Pokedex.instance.totalCaught)
        {
            case <= 30:
                multiplier = 0f;
                break;
            case <= 150:
                multiplier = .5f;
                break;
            case <= 300:
                multiplier = 1f;
                break;
            case <= 450:
                multiplier = 1.5f;
                break;
            case <= 600:
                multiplier = 2f;
                break;
            case >= 601:
                multiplier = 2.5f;
                break;
        }
        //switch statement based off the value
        //return based off threshold in switch statement
        float result = a * multiplier;
        //if there is a catching charm in the players inventory, double the result from the line above
        int c = (int)(result / 6f);
        int rng = GameManager.instance.rngSeed1.Next(0, 256);
        if (rng < c)
            return true;
        return false;
    }

    int CalculateDusk(Wild target)
    {
        float _override;

        if (System.DateTime.Now.Hour > 17 || System.DateTime.Now.Hour < 7)
            _override = 3.5f;
        else
            _override = 1f;

        if (GameManager.instance.ballHook == CharacterState.InCave)
                _override = 3.5f;

        return CalculateCatch(target, _override);
    }

    int CalculateDive(Wild target)
    {
        float _override;

        if (GameManager.instance.ballHook == CharacterState.Surfing)
            _override = 3.5f;
        else
            _override = 1f;

        return CalculateCatch(target, _override);
    }

    int CalculateDream(Wild target)
    {
        float _override;

        if (target.status == ConditionStatus.Asleep)
            _override = 4f;
        else
            _override = 1f;

        return CalculateCatch(target, _override);
    }

    int CalculateFast(Wild target)
    {
        float _override;

        if (target.pokemon.info.baseStats.speed >= 100)
            _override = 4f;
        else
            _override = 1f;

        return CalculateCatch(target, _override);
    }

    int CalculateHeavy(Wild target)//TODO Add pokemon weights for calculations
    {
        float _override;
        _override = 1f;
        return CalculateCatch(target, _override);
    }

    int CalculateLevel(Wild target, Owned owned)
    {
        float _override;

        int calc2 = target.level * 2;
        int calc4 = target.level * 4;

        if (owned.level <= target.level)
            _override = 1f;
        else if (owned.level > target.level && owned.level <= calc2)
            _override = 2f;
        else if (owned.level > calc2 && owned.level <= calc4)
            _override = 4f;
        else
            _override = 8f;

        return CalculateCatch(target, _override);
    }

    int CalculateLove(Wild target, Owned owned)
    {
        float _override;
        if (target.pokemon.info == owned.pokemon.info && GenderEvaluation(target.gender, owned.gender))
            _override = 8f;
        else
            _override = 1f;
        return CalculateCatch(target, _override);
    }

    bool GenderEvaluation(Gender targetG, Gender ownedG)
    {
        if (targetG == ownedG || targetG == Gender.None || targetG == Gender.Legendary || ownedG == Gender.None || ownedG == Gender.Legendary)
            return false;
        return true;
    }

    int CalculateMoon(Wild target)
    {
        float _override;

        if (target.pokemon.info.evolutionsByItem[0].evolutionItem.name == "Moon Stone")
            _override = 4f;
        else
            _override = 1f;

        return CalculateCatch(target, _override);
    }

    int CalculateNest(Wild target)
    {
        float _override;
        if (target.level > 29)
            _override = 1f;
        else
        {
            _override = (Mathf.Floor(((41f - target.level) * 4096f) / 10f)) / 4096f;
        }
        return CalculateCatch(target, _override);
    }

    int CalculateNet(Wild target)
    {
        float _override;
        if (target.pokemon.info.type1.type == CritterType.Bug || target.pokemon.info.type1.type == CritterType.Water || target.pokemon.info.type2.type == CritterType.Bug || target.pokemon.info.type2.type == CritterType.Water)
            _override = 3.5f;
        else
            _override = 1f;
        return CalculateCatch(target, _override);
    }

    int CalculateQuick(Wild target)
    {
        float _override;
        if (EncounterManager.instance.currentTurn == 0)
            _override = 5f;
        else
            _override = 1f;
        return CalculateCatch(target, _override);
    }

    int CalculateRepeat(Wild target)
    {
        float _override;
        if (Pokedex.instance.pokedex[0].entries[target.pokemon.info.pID - 1].obtained)
            _override = 3.5f;
        else
            _override = 1f;
        return CalculateCatch(target, _override);
    }

    int CalculateTimer(Wild target)
    {
        float _override;
        _override = Mathf.Min(1 + EncounterManager.instance.currentTurn * (1229 / 4096), 4f);
        return CalculateCatch(target, _override);
    }
}
public enum BallOverride { Regular, Masterball, Dive, Dream, Dusk, Fast, Heavy, Level, Love, Lure, Moon, Nest, Net, Quick, Repeat, Timer }